using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using budget_api.Models.CSV;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Models.Enums;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;
using CsvHelper;
using CsvHelper.Configuration;

namespace budget_api.Services
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _importRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public ImportService(
            IImportRepository importRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _importRepository = importRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<ImportBatchDto> UploadAsync(Guid walletId, Guid userId, IFormFile file)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId, userId)
                ?? throw new KeyNotFoundException("Wallet not found.");

            var records = ParseCsv(file);

            var existingHashes = await _importRepository.GetExistingHashesAsync(walletId);

            var batch = new ImportBatch
            {
                UserId = userId,
                WalletId = walletId,
                FileName = file.FileName,
                Status = ImportBatchStatus.Reviewing,
                TotalRows = records.Count,
                CreatedAt = DateTime.UtcNow
            };

            await _importRepository.CreateAsync(batch);

            int duplicateCount = 0;

            foreach (var record in records)
            {
                var date = ParseDate(record.Date);
                var txType = ParseTransactionType(record.Type);
                var hash = ComputeHash(walletId, date, record.Amount, record.RawDescription);
                var isDuplicate = existingHashes.Contains(hash);

                if (isDuplicate) duplicateCount++;

                var transaction = new Transaction
                {
                    WalletId = walletId,
                    UserId = userId,
                    Amount = record.Amount,
                    TransactionType = txType,
                    Date = date,
                    Description = record.Description,
                    RawDescription = record.RawDescription,
                    BankTxnType = record.BankTxnType,
                    CheckNumber = record.CheckNumber,
                    BalanceAfter = record.BalanceAfter,
                    DuplicateHash = hash,
                    Source = TransactionSource.Import,
                    ImportId = batch.Id,
                    IsDuplicate = isDuplicate,
                    ImportStatus = ImportStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _transactionRepository.CreateTransactionAsync(transaction);
            }

            batch.DuplicateCount = duplicateCount;
            await _importRepository.SaveChangesAsync();

            return _mapper.Map<ImportBatchDto>(batch);
        }

        public async Task<IEnumerable<ImportBatchDto>> GetBatchesAsync(Guid userId)
        {
            var batches = await _importRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ImportBatchDto>>(batches);
        }

        public async Task<ImportBatchDetailDto?> GetBatchDetailAsync(Guid batchId, Guid userId)
        {
            var batch = await _importRepository.GetByIdAsync(batchId, userId);
            if (batch == null) return null;

            var transactions = await _importRepository.GetBatchTransactionsAsync(batchId);

            var dto = _mapper.Map<ImportBatchDetailDto>(batch);
            dto.PendingDuplicates = transactions.Count(t => t.IsDuplicate && t.ImportStatus == ImportStatus.Pending);
            dto.Transactions = _mapper.Map<List<ImportTransactionDto>>(transactions);

            return dto;
        }

        public async Task BulkApproveAsync(Guid batchId, Guid userId, List<Guid> transactionIds)
        {
            var batch = await _importRepository.GetByIdAsync(batchId, userId)
                ?? throw new KeyNotFoundException("Batch not found.");

            if (batch.Status != ImportBatchStatus.Reviewing)
                throw new InvalidOperationException("Batch is no longer in review.");

            var transactions = await _importRepository.GetBatchTransactionsAsync(batchId);

            foreach (var tx in transactions.Where(t => transactionIds.Contains(t.Id) && t.IsDuplicate))
            {
                tx.ImportStatus = ImportStatus.Confirmed;
            }

            await _importRepository.SaveChangesAsync();
        }

        public async Task BulkRejectAsync(Guid batchId, Guid userId, List<Guid> transactionIds)
        {
            var batch = await _importRepository.GetByIdAsync(batchId, userId)
                ?? throw new KeyNotFoundException("Batch not found.");

            if (batch.Status != ImportBatchStatus.Reviewing)
                throw new InvalidOperationException("Batch is no longer in review.");

            var transactions = await _importRepository.GetBatchTransactionsAsync(batchId);

            foreach (var tx in transactions.Where(t => transactionIds.Contains(t.Id) && t.IsDuplicate))
            {
                tx.ImportStatus = ImportStatus.Rejected;
            }

            await _importRepository.SaveChangesAsync();
        }

        public async Task ConfirmAsync(Guid batchId, Guid userId)
        {
            var batch = await _importRepository.GetByIdAsync(batchId, userId)
                ?? throw new KeyNotFoundException("Batch not found.");

            if (batch.Status != ImportBatchStatus.Reviewing)
                throw new InvalidOperationException("Batch cannot be confirmed.");

            var wallet = await _walletRepository.GetWalletByIdAsync(batch.WalletId, userId)
                ?? throw new KeyNotFoundException("Wallet not found.");

            var transactions = await _importRepository.GetBatchTransactionsAsync(batchId);

            decimal balanceDelta = 0;
            int importedCount = 0;
            int skippedCount = 0;

            foreach (var tx in transactions)
            {
                if (tx.ImportStatus == ImportStatus.Rejected)
                {
                    tx.DeletedAt = DateTime.UtcNow;
                    skippedCount++;
                }
                else
                {
                    // Pending (non-duplicate auto-included) or Confirmed (reviewer-approved duplicate)
                    tx.ImportStatus = ImportStatus.Confirmed;
                    balanceDelta += tx.TransactionType == TransactionType.Income ? tx.Amount : -tx.Amount;
                    importedCount++;
                }
            }

            wallet.Balance += balanceDelta;
            wallet.UpdatedAt = DateTime.UtcNow;

            batch.Status = ImportBatchStatus.Confirmed;
            batch.ConfirmedAt = DateTime.UtcNow;
            batch.ImportedCount = importedCount;
            batch.SkippedCount = skippedCount;

            await _importRepository.SaveChangesAsync();
        }

        public async Task RollbackAsync(Guid batchId, Guid userId)
        {
            var batch = await _importRepository.GetByIdAsync(batchId, userId)
                ?? throw new KeyNotFoundException("Batch not found.");

            if (batch.Status == ImportBatchStatus.RolledBack)
                throw new InvalidOperationException("Batch already rolled back.");

            var transactions = await _importRepository.GetBatchTransactionsAsync(batchId);

            if (batch.Status == ImportBatchStatus.Confirmed)
            {
                var wallet = await _walletRepository.GetWalletByIdAsync(batch.WalletId, userId)
                    ?? throw new KeyNotFoundException("Wallet not found.");

                // Confirmed transactions are the only non-deleted ones at this point
                decimal reverseDelta = 0;
                foreach (var tx in transactions.Where(t => t.ImportStatus == ImportStatus.Confirmed))
                {
                    reverseDelta += tx.TransactionType == TransactionType.Income ? -tx.Amount : tx.Amount;
                }

                wallet.Balance += reverseDelta;
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            foreach (var tx in transactions)
            {
                tx.DeletedAt = DateTime.UtcNow;
            }

            batch.Status = ImportBatchStatus.RolledBack;
            batch.RolledBackAt = DateTime.UtcNow;

            await _importRepository.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static List<ImportCsvRecord> ParseCsv(IFormFile file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null,
            };

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<ImportCsvRecord>().ToList();
        }

        private static DateOnly ParseDate(string dateStr)
        {
            string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/M/yyyy" };
            foreach (var fmt in formats)
            {
                if (DateOnly.TryParseExact(dateStr.Trim(), fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return date;
            }
            throw new FormatException($"Unable to parse date: '{dateStr}'. Expected formats: yyyy-MM-dd, MM/dd/yyyy.");
        }

        private static TransactionType ParseTransactionType(string type)
        {
            return type.Trim().ToLowerInvariant() switch
            {
                "income" or "credit" => TransactionType.Income,
                "expense" or "debit" => TransactionType.Expense,
                _ => throw new FormatException($"Unknown transaction type: '{type}'. Expected Income or Expense.")
            };
        }

        private static string ComputeHash(Guid walletId, DateOnly date, decimal amount, string? rawDescription)
        {
            var input = $"{walletId}|{date:yyyy-MM-dd}|{amount}|{rawDescription?.ToLower().Trim() ?? ""}";
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
