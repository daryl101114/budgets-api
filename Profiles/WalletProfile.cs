using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile() 
        {
            CreateMap<Wallets, WalletCreationDto>().ReverseMap();
            CreateMap<Wallets, WalletsDto>().ReverseMap();
        }
    }
}
