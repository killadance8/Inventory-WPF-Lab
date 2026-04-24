using InventoryManager.Models;
using KamiLab2C_.Models;
using Microsoft.EntityFrameworkCore;

namespace KamiLab2C_.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=inventory.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();
        }
    }
}