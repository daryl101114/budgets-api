﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public enum TransactionType
    {
        Credit,
        Debit
    }
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("WalletId")]
        public Wallets? Wallet { get; set; }
        public Guid WalletId { get; set; }
        [Required]
        public string TransactionName { get; set; }
        public string? TransactionDescription { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRecurring { get; set; }
        public string TransactionType { get; set; }
        [ForeignKey("TransactionCategoryId")]
        public TransactionCategory? TransactionCategory { get; set; }
        public Guid TransactionCategoryId { get; set; }
    }
}
