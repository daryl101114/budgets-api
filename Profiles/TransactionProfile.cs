using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using System.Runtime.InteropServices;

namespace budget_api.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile() {
            CreateMap<Transaction, TransactionCreationDto>().ReverseMap();
            CreateMap<Transaction, TransactionUpdateDto>().ReverseMap();
            CreateMap<Transaction, TransactionsDto>().ReverseMap();
        }
    }
}
