using inventoryManager.Models;
using inventoryManager.Repositories;
using inventoryManager.Services;
using inventoryManager.ViewModels.Base;
using InventoryManager.Data;
using InventoryManager.Models;
using InventoryManager.Repositories;
using InventoryManager.Services;
using InventoryManager.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace inventoryManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IRepository<StockMovement> _movementRepository;
        private readonly ApplicationDbContext _context;

        private ObservableCollection<Category> _categories = new();
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<StockMovement> _movements = new();

        private Category? _selectedCategory;
        private Product? _selectedProduct;
        private StockMovement _newMovement;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<StockMovement> Movements
        {
            get => _movements;
            set => SetProperty(ref _movements, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                LoadProductsByCategory();
            }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                SetProperty(ref _selectedProduct, value);
                if (value != null)
                {
                    LoadProductMovements();
                    NewMovement.ProductId = value.Id;
                }
                else
                {
                    NewMovement.ProductId = 0;
                    Movements.Clear();
                }
            }
        }

        public StockMovement NewMovement
        {
            get => _newMovement;
            set => SetProperty(ref _newMovement, value);
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ProcessMovementCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainWindowViewModel()
        {
            _context = new ApplicationDbContext();
            _productRepository = new ProductRepository(_context);
            _categoryRepository = new Repository<Category>(_context);
            _movementRepository = new Repository<StockMovement>(_context);
            _inventoryService = new InventoryService(_movementRepository, _productRepository);

            _newMovement = new StockMovement
            {
                Date = DateTime.Now,
                Type = MovementType.In,
                Comment = string.Empty,
                ProductId = 0
            };

            AddCategoryCommand = new RelayCommand(async _ => await AddCategory());
            AddProductCommand = new RelayCommand(async _ => await AddProduct());
            EditProductCommand = new RelayCommand(async p => await EditProduct(p as Product), p => p != null);
            DeleteProductCommand = new RelayCommand(async p => await DeleteProduct(p as Product), p => p != null);
            ProcessMovementCommand = new RelayCommand(async _ => await ProcessMovement(), _ => CanProcessMovement());
            RefreshCommand = new RelayCommand(async _ => await LoadAllData());

            Task.Run(async () => await LoadAllData());
        }

        private bool CanProcessMovement()
        {
            return SelectedProduct != null &&
                   NewMovement.ProductId > 0 &&
                   NewMovement.Quantity > 0 &&
                   NewMovement.Quantity <= 1000000;
        }

        private async Task LoadAllData()
        {
            await LoadCategories();
            await LoadAllProducts();
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAllProducts()
        {
            try
            {
                var products = await _productRepository.GetProductsWithCategoryAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadProductsByCategory()
        {
            try
            {
                if (SelectedCategory == null)
                {
                    await LoadAllProducts();
                }
                else
                {
                    var allProducts = await _productRepository.GetProductsWithCategoryAsync();
                    var filtered = allProducts.Where(p => p.CategoryId == SelectedCategory.Id);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Products.Clear();
                        foreach (var product in filtered)
                        {
                            Products.Add(product);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadProductMovements()
        {
            if (SelectedProduct == null) return;

            try
            {
                var allMovements = await _movementRepository.GetAllAsync();
                var productMovements = allMovements
                    .Where(m => m.ProductId == SelectedProduct.Id)
                    .OrderByDescending(m => m.Date);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Movements.Clear();
                    foreach (var movement in productMovements)
                    {
                        Movements.Add(movement);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddCategory()
        {
            try
            {
                var dialog = new Views.CategoryDialog();
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.CategoryName))
                {
                    var newCategory = new Category { Name = dialog.CategoryName.Trim() };
                    await _categoryRepository.AddAsync(newCategory);
                    await _categoryRepository.SaveChangesAsync();
                    await LoadCategories();

                    MessageBox.Show("Категория успешно добавлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления категории: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddProduct()
        {
            try
            {
                var categoriesList = Categories.ToList();
                if (!categoriesList.Any())
                {
                    MessageBox.Show("Сначала создайте хотя бы одну категорию!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new Views.ProductDialog(categoriesList);
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    var isUnique = await _productRepository.IsSkuUniqueAsync(dialog.SKU);
                    if (!isUnique)
                    {
                        MessageBox.Show("Продукт с таким SKU уже существует!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newProduct = new Product
                    {
                        Name = dialog.ProductName,
                        SKU = dialog.SKU,
                        CategoryId = dialog.SelectedCategoryId,
                        Quantity = 0
                    };

                    await _productRepository.AddAsync(newProduct);
                    await _productRepository.SaveChangesAsync();
                    await LoadAllProducts();

                    MessageBox.Show("Продукт успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления продукта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditProduct(Product? product)
        {
            if (product == null) return;

            try
            {
                var categoriesList = Categories.ToList();
                var dialog = new Views.ProductDialog(categoriesList, product);
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    if (product.SKU != dialog.SKU)
                    {
                        var isUnique = await _productRepository.IsSkuUniqueAsync(dialog.SKU, product.Id);
                        if (!isUnique)
                        {
                            MessageBox.Show("Продукт с таким SKU уже существует!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    product.Name = dialog.ProductName;
                    product.SKU = dialog.SKU;
                    product.CategoryId = dialog.SelectedCategoryId;

                    await _productRepository.UpdateAsync(product);
                    await _productRepository.SaveChangesAsync();
                    await LoadAllProducts();

                    MessageBox.Show("Продукт успешно обновлён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования продукта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteProduct(Product? product)
        {
            if (product == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить продукт '{product.Name}'?\n\n" +
                "ВНИМАНИЕ: Будут удалены все связанные операции!",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _productRepository.DeleteAsync(product.Id);
                    await _productRepository.SaveChangesAsync();
                    await LoadAllProducts();

                    if (SelectedProduct?.Id == product.Id)
                    {
                        SelectedProduct = null;
                        Movements.Clear();
                    }

                    MessageBox.Show("Продукт успешно удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления продукта: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task ProcessMovement()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Выберите товар для выполнения операции!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewMovement.Date = DateTime.Now;
            NewMovement.ProductId = SelectedProduct.Id;

            try
            {
                var result = await _inventoryService.ProcessWithResultAsync(NewMovement);

                if (result.Success)
                {
                    await LoadAllProducts();
                    LoadProductMovements();

                    _newMovement = new StockMovement
                    {
                        Date = DateTime.Now,
                        Type = MovementType.In,
                        Comment = string.Empty,
                        ProductId = SelectedProduct.Id
                    };
                    OnPropertyChanged(nameof(NewMovement));

                    MessageBox.Show($"Движение успешно выполнено! Новый остаток: {result.NewQuantity} ед.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    SelectedProduct = Products.FirstOrDefault(p => p.Id == SelectedProduct.Id);
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения операции: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Cleanup()
        {
            _context?.Dispose();
        }
    }
}