using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public class WalletType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Type { get; set; }
        public ICollection<Wallets> Wallets { get; set; } = new List<Wallets>();
    }
}
