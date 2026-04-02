using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;

namespace budget_api.Services
{
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly ITransactionCategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public TransactionCategoryService(ITransactionCategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionCategoryDto>> GetCategoriesAsync(Guid userId)
        {
            var categories = await _categoryRepository.GetCategoriesAsync(userId);
            return _mapper.Map<IEnumerable<TransactionCategoryDto>>(categories);
        }

        public async Task<TransactionCategoryDto?> GetCategoryByIdAsync(Guid categoryId, Guid userId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) return null;
            // Only visible if system category or belongs to the requesting user
            if (category.UserId != null && category.UserId != userId) return null;
            return _mapper.Map<TransactionCategoryDto>(category);
        }

        public async Task CreateCategoryAsync(CreateTransactionCategoryDto dto, Guid userId)
        {
            var category = _mapper.Map<TransactionCategory>(dto);
            category.UserId = userId;
            category.CreatedAt = DateTime.UtcNow;
            await _categoryRepository.CreateCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Guid categoryId, Guid userId, UpdateTransactionCategoryDto dto)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("System categories cannot be modified.");

            _mapper.Map(dto, category);
            await _categoryRepository.UpdateCategoryAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId, Guid userId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("System categories cannot be deleted.");

            await _categoryRepository.SoftDeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
