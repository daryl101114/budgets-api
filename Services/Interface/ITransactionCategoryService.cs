using budget_api.Models.DTOs;

namespace budget_api.Services.Interface
{
    public interface ITransactionCategoryService
    {
        Task<IEnumerable<TransactionCategoryDto>> GetCategoriesAsync(Guid userId);
        Task<TransactionCategoryDto?> GetCategoryByIdAsync(Guid categoryId, Guid userId);
        Task CreateCategoryAsync(CreateTransactionCategoryDto dto, Guid userId);
        Task UpdateCategoryAsync(Guid categoryId, Guid userId, UpdateTransactionCategoryDto dto);
        Task DeleteCategoryAsync(Guid categoryId, Guid userId);
    }
}
