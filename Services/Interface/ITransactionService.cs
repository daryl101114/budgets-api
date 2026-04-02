using budget_api.Models.DTOs;

namespace budget_api.Services.Interface
{
    public interface ITransactionService
    {
        Task CreateTransactionAsync(CreateTransactionDto createTransactionDto, Guid userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByWalletAsync(Guid walletId, Guid userId);
        Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId);
        Task UpdateTransactionAsync(Guid transactionId, Guid userId, UpdateTransactionDto updateTransactionDto);
        Task DeleteTransactionAsync(Guid transactionId, Guid userId);
    }
}
