using budget_api.Models.DTOs;
using budget_api.Models.Pagination;

namespace budget_api.Services.Interface
{
    public interface ITransactionService
    {
        Task CreateTransactionAsync(CreateTransactionDto createTransactionDto, Guid userId);
        Task<CursorPagedResult<TransactionDto>> GetTransactionsByWalletAsync(
            Guid walletId, Guid userId, int limit, string? cursor);
        Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId);
        Task UpdateTransactionAsync(Guid transactionId, Guid userId, UpdateTransactionDto updateTransactionDto);
        Task DeleteTransactionAsync(Guid transactionId, Guid userId);
    }
}
