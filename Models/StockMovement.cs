using InventoryManager.Models;
using System;

namespace KamiLab2C_.Models
{
    public enum MovementType
    {
        In,
        Out
    }

    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public MovementType Type { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}