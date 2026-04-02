using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;

namespace budget_api.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public WalletService(IWalletRepository walletRepository, IUserRepository userRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId)
        {
            var wallet = _mapper.Map<Wallet>(createWalletDto);
            wallet.UserId = userId;
            await _walletRepository.CreateWalletAsync(wallet);
            await _walletRepository.SaveChangesAsync();
        }

        public async Task<UserWalletsDto?> GetUserWalletsAsync(Guid userId)
        {
            var user = await _userRepository.GetUserWalletsAsync(userId);
            return _mapper.Map<UserWalletsDto>(user);
        }

        public async Task UpdateWalletAsync(Guid walletId, Guid userId, UpdateWalletDto updateWalletDto)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId, userId)
                ?? throw new KeyNotFoundException($"Wallet {walletId} not found.");

            _mapper.Map(updateWalletDto, wallet);
            await _walletRepository.UpdateWalletAsync(wallet);
            await _walletRepository.SaveChangesAsync();
        }

        public async Task DeleteWalletAsync(Guid walletId, Guid userId)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId, userId)
                ?? throw new KeyNotFoundException($"Wallet {walletId} not found.");

            await _walletRepository.SoftDeleteWalletAsync(wallet);
            await _walletRepository.SaveChangesAsync();
        }
    }
}
