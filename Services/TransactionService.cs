using AutoMapper;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper) {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task CreateTransactionAsync(TransactionCreationDto transactionCreationDto)
        {
            //Map Values
            var transaction = _mapper.Map<Transaction>(transactionCreationDto);
            await _transactionRepository.CreateTransactionAsync(transaction);
            await _transactionRepository.SaveTransactionAsync();
        }

        public async Task<IEnumerable<TransactionCategory>> GetTransactionCategoriesAsync()
        {
            return await _transactionRepository.GetTransactionCategoriesAsync();
        }

        public async Task<IEnumerable<TransactionsDto>> GetTransactionsAsync(Guid walletId)
        {
            var transactionsList = await _transactionRepository.GetTransactionsAsync(walletId);
            var mappedTransactions = new List<TransactionsDto>();
            //Loop and map
            foreach (var item in transactionsList)
            {
                var mapped = _mapper.Map<TransactionsDto>(item);
                mappedTransactions.Add(mapped);
            }
            return mappedTransactions;

        }
    }
}
