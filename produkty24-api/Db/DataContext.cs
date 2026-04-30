using Microsoft.EntityFrameworkCore;
using Produkty24_API.Models;
using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Db
{
    public class DataContext : DbContext
    {
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<CountryEntity> Countries { get; set; }
        public DbSet<ShippingMethodEntity> ShippingMethods { get; set; }
        public DbSet<ManufacturerEntity> Manufacturers { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<StockItemEntity> StockItems { get; set; }
        public DbSet<StockArrivalEntity> StockArrivals { get; set; }
        public DbSet<OrderItemEntity> OrdersItems { get; set; }
        public DbSet<ExchangeRateEntity> ExchangeRates { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderStatusEntity> OrderStatuses { get; set; }
        public DbSet<PaymentEntity> Payments { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClientEntity>().Navigation(e => e.Country).AutoInclude();
            modelBuilder.Entity<ClientEntity>().Navigation(e => e.ShippingMethod).AutoInclude();
            modelBuilder.Entity<StockItemEntity>().Navigation(e => e.Manufacturer).AutoInclude();
            modelBuilder.Entity<StockItemEntity>().Navigation(e => e.Currency).AutoInclude();
            modelBuilder.Entity<StockArrivalEntity>().Navigation(e => e.StockItem).AutoInclude();
            modelBuilder.Entity<ExchangeRateEntity>().Navigation(e => e.Currency).AutoInclude();
            modelBuilder.Entity<OrderEntity>().Navigation(e => e.Client).AutoInclude();
            modelBuilder.Entity<OrderEntity>().Navigation(e => e.Status).AutoInclude();
            modelBuilder.Entity<OrderItemEntity>().Navigation(e => e.Order).AutoInclude();
            modelBuilder.Entity<OrderItemEntity>().Navigation(e => e.StockItem).AutoInclude();
            modelBuilder.Entity<PaymentEntity>().Navigation(e => e.Client).AutoInclude();
            modelBuilder.Entity<PaymentEntity>().Navigation(e => e.Order).AutoInclude();

            modelBuilder.Seed();
        }
    }
}
