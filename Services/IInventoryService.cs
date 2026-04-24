using InventoryManager.Models;
using System.Threading.Tasks;

namespace InventoryManager.Services
{
    public interface IInventoryService
    {
        Task<bool> ProcessMovementAsync(StockMovement movement);
        Task<int> GetCurrentStockAsync(int productId);
        Task<bool> ValidateMovementAsync(StockMovement movement);
        Task<StockMovementResult> ProcessWithResultAsync(StockMovement movement);
    }

    public class StockMovementResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int NewQuantity { get; set; }
    }
}