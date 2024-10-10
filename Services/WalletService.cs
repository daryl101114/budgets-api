using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
                    
        public WalletService(IWalletRepository walletRepository, IMapper mapper, IUserRepository userRepository) 
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        

        public async Task CreateWallet(WalletCreationDto walletCreationDto, Guid userId)
        {
            //Map Wallet
            var wallet = _mapper.Map<Wallets>(walletCreationDto);
            wallet.UserId = userId;
            //Create wallet
            await _walletRepository.CreateWalletAsync(wallet);
            //Save Changes to DB
            await _walletRepository.SaveChangesAsync();
        }

        public async Task<UserWalletsDto?> GetUserWalletsAsync(Guid userId)
        {
            var userWallets = await _userRepository.GetUserWalletsAsync(userId);
            var mappedUserWallet = _mapper.Map<UserWalletsDto>(userWallets);
            return mappedUserWallet;
        }
    }
}
