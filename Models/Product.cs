using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название продукта обязательно")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU обязателен")]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}