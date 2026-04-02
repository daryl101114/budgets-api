using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<Wallet, CreateWalletDto>().ReverseMap();
            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<UpdateWalletDto, Wallet>()
                .ForMember(dest => dest.Currency, opt => {
                    opt.PreCondition(src => src.Currency.HasValue);
                    opt.MapFrom(src => src.Currency!.Value);
                })
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
