using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Models.Enums;
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

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByWalletAsync(Guid walletId, Guid userId)
        {
            var transactions = await _transactionRepository.GetTransactionsByWalletAsync(walletId, userId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
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
