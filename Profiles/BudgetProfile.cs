using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class BudgetProfile : Profile
    {
        public BudgetProfile()
        {
            CreateMap<Budget, BudgetDto>();

            CreateMap<CreateBudgetDto, Budget>();

            CreateMap<UpdateBudgetDto, Budget>()
                .ForMember(dest => dest.TotalIncome, opt => {
                    opt.PreCondition(src => src.TotalIncome.HasValue);
                    opt.MapFrom(src => src.TotalIncome!.Value);
                })
                .ForMember(dest => dest.StartDate, opt => {
                    opt.PreCondition(src => src.StartDate.HasValue);
                    opt.MapFrom(src => src.StartDate!.Value);
                })
                .ForMember(dest => dest.EndDate, opt => {
                    opt.PreCondition(src => src.EndDate.HasValue);
                    opt.MapFrom(src => src.EndDate!.Value);
                })
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<BudgetAllocation, BudgetAllocationDto>()
                .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.Bucket != null ? src.Bucket.BucketName : string.Empty));
        }
    }
}
