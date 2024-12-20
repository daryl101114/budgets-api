﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public class Wallets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid UserId { get; set; } = new Guid();
        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; }
        [ForeignKey("WalletTypeId")]
        public WalletType? WalletType { get; set; }
        public int? WalletTypeId { get; set; }
        public double? Balance { get; set; } = 0.00;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Currency { get; set; }
        public string? Emoji { get; set; }
    }
}
