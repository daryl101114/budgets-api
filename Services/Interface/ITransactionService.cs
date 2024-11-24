using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Services.Interface
{
    public interface ITransactionService
    {
        Task CreateTransactionAsync(TransactionCreationDto transactionCreationDto);
        Task<IEnumerable<TransactionCategory>> GetTransactionCategoriesAsync();
        Task<IEnumerable<TransactionsDto>> GetTransactionsAsync(Guid walletId);
    }
}
