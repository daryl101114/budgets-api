using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Profiles
{
    public class ImportProfile : Profile
    {
        public ImportProfile()
        {
            CreateMap<ImportBatch, ImportBatchDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<ImportBatch, ImportBatchDetailDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PendingDuplicates, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<Transaction, ImportTransactionDto>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.ToString()))
                .ForMember(dest => dest.ImportStatus, opt => opt.MapFrom(src => src.ImportStatus.HasValue ? src.ImportStatus.Value.ToString() : string.Empty));
        }
    }
}
