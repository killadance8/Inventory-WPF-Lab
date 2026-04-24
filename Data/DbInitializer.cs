using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;

namespace InventoryManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            try
            {
                using var context = new ApplicationDbContext();

                // Автоматическое создание базы данных и применение миграций
                context.Database.EnsureCreated();

                // Проверяем, есть ли уже данные
                if (!context.Categories.Any())
                {
                    // Добавляем начальные категории
                    SeedInitialData(context);
                }

                System.Diagnostics.Debug.WriteLine("База данных инициализирована успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}\n\n" +
                    "Приложение может работать некорректно.",
                    "Ошибка БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private static void SeedInitialData(ApplicationDbContext context)
        {
            // Добавляем тестового пользователя
            context.Users.Add(new Models.User
            {
                Name = "Системный администратор"
            });

            // Добавляем тестовые категории
            var electronics = new Models.Category { Name = "Электроника" };
            var furniture = new Models.Category { Name = "Мебель" };
            var office = new Models.Category { Name = "Канцелярия" };

            context.Categories.AddRange(electronics, furniture, office);
            context.SaveChanges();

            // Добавляем тестовые продукты
            var products = new[]
            {
                new Models.Product { Name = "Ноутбук Lenovo", SKU = "NB-001", CategoryId = electronics.Id, Quantity = 10 },
                new Models.Product { Name = "Мышь Logitech", SKU = "MS-001", CategoryId = electronics.Id, Quantity = 25 },
                new Models.Product { Name = "Стул офисный", SKU = "CH-001", CategoryId = furniture.Id, Quantity = 5 },
                new Models.Product { Name = "Бумага A4", SKU = "PP-001", CategoryId = office.Id, Quantity = 100 },
                new Models.Product { Name = "Ручка шариковая", SKU = "PN-001", CategoryId = office.Id, Quantity = 200 }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // Добавляем тестовые движения
            var movements = new[]
            {
                new Models.StockMovement { ProductId = products[0].Id, Type = Models.MovementType.In, Quantity = 10, Date = DateTime.Now.AddDays(-5), Comment = "Начальный остаток" },
                new Models.StockMovement { ProductId = products[1].Id, Type = Models.MovementType.In, Quantity = 25, Date = DateTime.Now.AddDays(-5), Comment = "Начальный остаток" },
                new Models.StockMovement { ProductId = products[2].Id, Type = Models.MovementType.In, Quantity = 5, Date = DateTime.Now.AddDays(-5), Comment = "Начальный остаток" },
                new Models.StockMovement { ProductId = products[3].Id, Type = Models.MovementType.In, Quantity = 100, Date = DateTime.Now.AddDays(-5), Comment = "Начальный остаток" },
                new Models.StockMovement { ProductId = products[4].Id, Type = Models.MovementType.In, Quantity = 200, Date = DateTime.Now.AddDays(-5), Comment = "Начальный остаток" }
            };

            context.StockMovements.AddRange(movements);
            context.SaveChanges();
        }
    }
}