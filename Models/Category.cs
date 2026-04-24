using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название категории обязательно")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}