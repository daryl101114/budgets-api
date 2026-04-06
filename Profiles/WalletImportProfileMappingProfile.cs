using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class WalletImportProfileMappingProfile : Profile
    {
        public WalletImportProfileMappingProfile()
        {
            CreateMap<WalletImportProfile, WalletImportProfileDto>();
            CreateMap<UpsertWalletImportProfileDto, WalletImportProfile>();
        }
    }
}
