using InventoryManager.Models;
using InventoryManager.Repositories;
using System.Threading.Tasks;

namespace InventoryManager.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<StockMovement> _movementRepository;
        private readonly IProductRepository _productRepository;

        public InventoryService(
            IRepository<StockMovement> movementRepository,
            IProductRepository productRepository)
        {
            _movementRepository = movementRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> ProcessMovementAsync(StockMovement movement)
        {
            var result = await ProcessWithResultAsync(movement);
            return result.Success;
        }

        public async Task<StockMovementResult> ProcessWithResultAsync(StockMovement movement)
        {
            var result = new StockMovementResult();

            // Получаем продукт
            var product = await _productRepository.GetByIdAsync(movement.ProductId);
            if (product == null)
            {
                result.ErrorMessage = "Продукт не найден";
                return result;
            }

            // Валидация движения
            if (!await ValidateMovementAsync(movement))
            {
                result.ErrorMessage = movement.Type == MovementType.Out && product.Quantity < movement.Quantity
                    ? $"Недостаточно товара на складе. Доступно: {product.Quantity}"
                    : "Некорректные данные движения";
                return result;
            }

            // Обновляем остаток
            if (movement.Type == MovementType.In)
            {
                product.Quantity += movement.Quantity;
            }
            else
            {
                product.Quantity -= movement.Quantity;
            }

            // Сохраняем изменения
            await _productRepository.UpdateAsync(product);
            await _movementRepository.AddAsync(movement);
            await _movementRepository.SaveChangesAsync();

            result.Success = true;
            result.NewQuantity = product.Quantity;
            return result;
        }

        public async Task<int> GetCurrentStockAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product?.Quantity ?? 0;
        }

        public async Task<bool> ValidateMovementAsync(StockMovement movement)
        {
            if (movement.Quantity <= 0)
                return false;

            if (movement.Type == MovementType.Out)
            {
                var currentStock = await GetCurrentStockAsync(movement.ProductId);
                if (currentStock < movement.Quantity)
                    return false;
            }

            return true;
        }
    }
}