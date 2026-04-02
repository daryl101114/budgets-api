using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<CreateTransactionDto, Transaction>();
            CreateMap<Transaction, TransactionDto>();
            CreateMap<UpdateTransactionDto, Transaction>()
                .ForMember(dest => dest.Date, opt => {
                    opt.PreCondition(src => src.Date.HasValue);
                    opt.MapFrom(src => src.Date!.Value);
                })
                .ForMember(dest => dest.Amount, opt => {
                    opt.PreCondition(src => src.Amount.HasValue);
                    opt.MapFrom(src => src.Amount!.Value);
                })
                .ForMember(dest => dest.TransactionType, opt => {
                    opt.PreCondition(src => src.TransactionType.HasValue);
                    opt.MapFrom(src => src.TransactionType!.Value);
                })
                .ForMember(dest => dest.SubcategoryId, opt => {
                    opt.PreCondition(src => src.SubcategoryId.HasValue);
                    opt.MapFrom(src => src.SubcategoryId!.Value);
                })
                .ForMember(dest => dest.IsRecurring, opt => {
                    opt.PreCondition(src => src.IsRecurring.HasValue);
                    opt.MapFrom(src => src.IsRecurring!.Value);
                })
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
