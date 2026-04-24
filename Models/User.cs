using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}