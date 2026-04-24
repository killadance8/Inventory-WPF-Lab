using Microsoft.EntityFrameworkCore;
using InventoryManager.Data;
using InventoryManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManager.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategoryAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithMovementsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.StockMovements.OrderByDescending(m => m.Date))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return false;

            var query = _dbSet.Where(p => p.SKU == sku);
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return !await query.AnyAsync();
        }
    }
}