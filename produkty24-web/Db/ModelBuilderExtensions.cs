using Microsoft.EntityFrameworkCore;
using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.Db
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryEntity>()
                .HasData(
                   new CountryEntity { Id = 1, Name = "Україна" },
                   new CountryEntity { Id = 2, Name = "Молдова" },
                   new CountryEntity { Id = 3, Name = "Польща" }
            );

            modelBuilder.Entity<CurrencyEntity>()
                .HasData(
                   new CurrencyEntity { Id = 1, Code = "EUR" },
                   new CurrencyEntity { Id = 2, Code = "USD" },
                   new CurrencyEntity { Id = 3, Code = "UAH" }
            );

            modelBuilder.Entity<ShippingMethodEntity>()
                .HasData(
                   new ShippingMethodEntity { Id = 1, Name = "Нова пошта" },
                   new ShippingMethodEntity { Id = 2, Name = "Укрпошта" },
                   new ShippingMethodEntity { Id = 3, Name = "Самовивіз" }
            );

            modelBuilder.Entity<OrderStatusEntity>()
                .HasData(
                   new OrderStatusEntity { Id = 1, Name = "Готово" },
                   new OrderStatusEntity { Id = 2, Name = "До відправки" },
                   new OrderStatusEntity { Id = 3, Name = "Оплачено повністю" },
                   new OrderStatusEntity { Id = 4, Name = "НОВИЙ" },
                   new OrderStatusEntity { Id = 5, Name = "Виставлено рахунок" },
                   new OrderStatusEntity { Id = 6, Name = "Оплачено частково" },
                   new OrderStatusEntity { Id = 7, Name = "Відправлено" }
            );

            modelBuilder.Entity<ExchangeRateEntity>()
                .HasData(
                   new ExchangeRateEntity { Id = 1, Date = new DateTime(2000, 01, 01), CurrencyId = 3, Value = 1 }
            );
        }
    }
}
