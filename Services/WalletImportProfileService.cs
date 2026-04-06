using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;

namespace budget_api.Services
{
    public class WalletImportProfileService : IWalletImportProfileService
    {
        private readonly IWalletImportProfileRepository _profileRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public WalletImportProfileService(
            IWalletImportProfileRepository profileRepository,
            IWalletRepository walletRepository,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<WalletImportProfileDto?> GetByWalletIdAsync(Guid walletId, Guid userId)
        {
            await EnsureWalletOwnership(walletId, userId);
            var profile = await _profileRepository.GetByWalletIdAsync(walletId);
            return profile == null ? null : _mapper.Map<WalletImportProfileDto>(profile);
        }

        public async Task<WalletImportProfileDto> UpsertAsync(Guid walletId, Guid userId, UpsertWalletImportProfileDto dto)
        {
            await EnsureWalletOwnership(walletId, userId);

            var existing = await _profileRepository.GetByWalletIdAsync(walletId);

            if (existing != null)
            {
                _mapper.Map(dto, existing);
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                existing = _mapper.Map<WalletImportProfile>(dto);
                existing.WalletId = walletId;
                existing.CreatedAt = DateTime.UtcNow;
                existing.UpdatedAt = DateTime.UtcNow;
                await _profileRepository.AddAsync(existing);
            }

            await _profileRepository.SaveChangesAsync();
            return _mapper.Map<WalletImportProfileDto>(existing);
        }

        public async Task DeleteAsync(Guid walletId, Guid userId)
        {
            await EnsureWalletOwnership(walletId, userId);

            var profile = await _profileRepository.GetByWalletIdAsync(walletId)
                ?? throw new KeyNotFoundException("No import profile found for this wallet.");

            _profileRepository.Delete(profile);
            await _profileRepository.SaveChangesAsync();
        }

        private async Task EnsureWalletOwnership(Guid walletId, Guid userId)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId, userId)
                ?? throw new KeyNotFoundException("Wallet not found.");
        }
    }
}
