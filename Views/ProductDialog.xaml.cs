using InventoryManager.Models;
using KamiLab2C_.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace KamiLab2C_
{
    public partial class ProductDialog : Window
    {
        public string ProductName { get; private set; }
        public string SKU { get; private set; }
        public int SelectedCategoryId { get; private set; }

        public ProductDialog(List<Category> categories)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            CategoryCombo.ItemsSource = categories;
            if (categories.Any())
                CategoryCombo.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameBox.Text))
            {
                MessageBox.Show("Введите название!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SKUBox.Text))
            {
                MessageBox.Show("Введите SKU!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductName = ProductNameBox.Text.Trim();
            SKU = SKUBox.Text.Trim().ToUpper();
            SelectedCategoryId = (int)CategoryCombo.SelectedValue;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}