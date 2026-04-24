using InventoryManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManager.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
        Task<Product?> GetProductWithMovementsAsync(int id);
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
    }
}