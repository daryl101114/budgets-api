﻿using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[Controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;
        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService) 
        {
            _logger = logger;
            _transactionService = transactionService;
        }
        [HttpPost("createTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody]TransactionCreationDto transactionCreationDto)
        {
            try
            {
                await _transactionService.CreateTransactionAsync(transactionCreationDto);
                return Ok("Transaction created");
            }
            catch (Exception ex)
            {
                _logger.LogError("At Create Transaction", ex);
                return StatusCode(500, "Failed to create transaction");
            }

        }

        [HttpGet("TransactionCategories")]
        public async Task<IActionResult> GetTransactionCategories()
        {
            try
            {
                return Ok(await _transactionService.GetTransactionCategoriesAsync());
            }
            catch(Exception ex)
            {
                _logger.LogError("At Get Transaction Categories", ex);
                return StatusCode(500, "Failed to get Transaction Categories");
            }
        }
        [HttpGet("transactions/{walletId}")]
        public async Task<IActionResult> GetWalletTransactions(Guid walletId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsAsync(walletId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retreive transactions for {walletId}", ex);
                return StatusCode(500, "Failed to get Transactions");
            }
        }

    }
}
