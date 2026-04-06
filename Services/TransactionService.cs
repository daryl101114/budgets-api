using System.Globalization;
using System.Text;
using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Models.Enums;
using budget_api.Models.Pagination;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;

namespace budget_api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task CreateTransactionAsync(CreateTransactionDto createTransactionDto, Guid userId)
        {
            var transaction = _mapper.Map<Transaction>(createTransactionDto);
            transaction.UserId = userId;
            transaction.Source = TransactionSource.Manual;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _transactionRepository.CreateTransactionAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
        }

        public async Task<CursorPagedResult<TransactionDto>> GetTransactionsByWalletAsync(
            Guid walletId, Guid userId, int limit, string? cursor)
        {
            limit = Math.Clamp(limit, 1, 100);

            (DateOnly? cursorDate, DateTime? cursorCreatedAt) = DecodeCursor(cursor);

            // Fetch one extra to determine if there are more pages
            var rows = await _transactionRepository.GetTransactionsByWalletAsync(
                walletId, userId, limit + 1, cursorDate, cursorCreatedAt);

            var hasMore = rows.Count > limit;
            var page = hasMore ? rows.Take(limit).ToList() : rows;

            var lastItem = page.LastOrDefault();
            var nextCursor = hasMore && lastItem != null
                ? EncodeCursor(lastItem.Date, lastItem.CreatedAt)
                : null;

            return new CursorPagedResult<TransactionDto>
            {
                Data = _mapper.Map<List<TransactionDto>>(page),
                NextCursor = nextCursor,
                HasMore = hasMore
            };
        }

        private static string EncodeCursor(DateOnly date, DateTime createdAt)
        {
            var raw = $"{date:yyyy-MM-dd}|{createdAt:O}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        }

        private static (DateOnly? date, DateTime? createdAt) DecodeCursor(string? cursor)
        {
            if (string.IsNullOrWhiteSpace(cursor)) return (null, null);

            try
            {
                var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var parts = raw.Split('|');
                var date = DateOnly.Parse(parts[0], CultureInfo.InvariantCulture);
                var createdAt = DateTime.Parse(parts[1], null, DateTimeStyles.RoundtripKind);
                return (date, createdAt);
            }
            catch
            {
                throw new FormatException("Invalid cursor.");
            }
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId, userId);
            return transaction == null ? null : _mapper.Map<TransactionDto>(transaction);
        }

        public async Task UpdateTransactionAsync(Guid transactionId, Guid userId, UpdateTransactionDto updateTransactionDto)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId, userId)
                ?? throw new KeyNotFoundException($"Transaction {transactionId} not found.");

            _mapper.Map(updateTransactionDto, transaction);
            await _transactionRepository.UpdateTransactionAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Guid transactionId, Guid userId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId, userId)
                ?? throw new KeyNotFoundException($"Transaction {transactionId} not found.");

            await _transactionRepository.SoftDeleteAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
        }
    }
}
