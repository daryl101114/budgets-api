using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class TransactionCategoryProfile : Profile
    {
        public TransactionCategoryProfile()
        {
            CreateMap<TransactionCategory, TransactionCategoryDto>();
            CreateMap<CreateTransactionCategoryDto, TransactionCategory>();
            CreateMap<UpdateTransactionCategoryDto, TransactionCategory>()
                .ForMember(dest => dest.BucketId, opt => {
                    opt.PreCondition(src => src.BucketId.HasValue);
                    opt.MapFrom(src => src.BucketId!.Value);
                })
                .ForMember(dest => dest.IsFixed, opt => {
                    opt.PreCondition(src => src.IsFixed.HasValue);
                    opt.MapFrom(src => src.IsFixed!.Value);
                })
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
