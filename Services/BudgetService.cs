using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;

namespace budget_api.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public BudgetService(IBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BudgetDto>> GetBudgetsAsync(Guid userId)
        {
            var budgets = await _budgetRepository.GetBudgetsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<BudgetDto>>(budgets);
        }

        public async Task<BudgetDto?> GetBudgetByIdAsync(Guid budgetId, Guid userId)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId);
            return budget == null ? null : _mapper.Map<BudgetDto>(budget);
        }

        public async Task<BudgetDto?> GetActiveBudgetAsync(Guid userId)
        {
            var budget = await _budgetRepository.GetActiveBudgetAsync(userId);
            return budget == null ? null : _mapper.Map<BudgetDto>(budget);
        }

        public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto dto, Guid userId)
        {
            var budget = _mapper.Map<Budget>(dto);
            budget.UserId = userId;
            budget.CreatedAt = DateTime.UtcNow;
            budget.UpdatedAt = DateTime.UtcNow;

            if (dto.IsActive)
                await _budgetRepository.DeactivateAllBudgetsAsync(userId);

            await _budgetRepository.CreateBudgetAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            return _mapper.Map<BudgetDto>(budget);
        }

        public async Task UpdateBudgetAsync(Guid budgetId, Guid userId, UpdateBudgetDto dto)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId)
                ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

            _mapper.Map(dto, budget);
            budget.UpdatedAt = DateTime.UtcNow;

            // Recalculate allocated amounts if TotalIncome changed
            if (dto.TotalIncome.HasValue)
            {
                foreach (var allocation in budget.Allocations)
                    allocation.AllocatedAmount = Math.Round(allocation.Percentage / 100m * budget.TotalIncome, 2);
            }

            await _budgetRepository.SaveChangesAsync();
        }

        public async Task ActivateBudgetAsync(Guid budgetId, Guid userId)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId)
                ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

            await _budgetRepository.DeactivateAllBudgetsAsync(userId);
            budget.IsActive = true;
            budget.UpdatedAt = DateTime.UtcNow;
            await _budgetRepository.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(Guid budgetId, Guid userId)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId)
                ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

            await _budgetRepository.SoftDeleteAsync(budget);
            await _budgetRepository.SaveChangesAsync();
        }

        public async Task AddAllocationAsync(Guid budgetId, Guid userId, CreateBudgetAllocationDto dto)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId)
                ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

            var currentTotal = await _budgetRepository.GetTotalPercentageAsync(budgetId);
            if (currentTotal + dto.Percentage > 100m)
                throw new InvalidOperationException(
                    $"Adding {dto.Percentage}% would exceed 100%. Currently allocated: {currentTotal}%.");

            var allocation = new BudgetAllocation
            {
                BudgetId = budgetId,
                BucketId = dto.BucketId,
                Percentage = dto.Percentage,
                AllocatedAmount = Math.Round(dto.Percentage / 100m * budget.TotalIncome, 2)
            };

            await _budgetRepository.AddAllocationAsync(allocation);
            await _budgetRepository.SaveChangesAsync();
        }

        public async Task UpdateAllocationAsync(Guid budgetId, Guid allocationId, Guid userId, UpdateBudgetAllocationDto dto)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId)
                ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

            var allocation = await _budgetRepository.GetAllocationByIdAsync(allocationId, budgetId)
                ?? throw new KeyNotFoundException($"Allocation {allocationId} not found.");

            if (dto.Percentage.HasValue)
            {
                var otherTotal = await _budgetRepository.GetTotalPercentageAsync(budgetId, allocationId);
                if (otherTotal + dto.Percentage.Value > 100m)
                    throw new InvalidOperationException(
                        $"Setting {dto.Percentage}% would exceed 100%. Other allocations total: {otherTotal}%.");

                allocation.Percentage = dto.Percentage.Value;
                allocation.AllocatedAmount = Math.Round(dto.Percentage.Value / 100m * budget.TotalIncome, 2);
            }

            if (dto.BucketId.HasValue)
                allocation.BucketId = dto.BucketId.Value;

            await _budgetRepository.SaveChangesAsync();
        }

        public async Task RemoveAllocationAsync(Guid budgetId, Guid allocationId, Guid userId)
        {
            _ = await _budgetRepository.GetBudgetByIdAsync(budgetId, userId)
                ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

            var allocation = await _budgetRepository.GetAllocationByIdAsync(allocationId, budgetId)
                ?? throw new KeyNotFoundException($"Allocation {allocationId} not found.");

            await _budgetRepository.RemoveAllocationAsync(allocation);
            await _budgetRepository.SaveChangesAsync();
        }
    }
}
