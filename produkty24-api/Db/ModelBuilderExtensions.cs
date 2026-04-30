using Microsoft.EntityFrameworkCore;
using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Db
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryEntity>()
                .HasData(
                   new CountryEntity { Id = 1, Name = "Украина" },
                   new CountryEntity { Id = 2, Name = "Молдова" },
                   new CountryEntity { Id = 3, Name = "Польша" }
            );

            modelBuilder.Entity<CurrencyEntity>()
                .HasData(
                   new CurrencyEntity { Id = 1, Code = "EUR" },
                   new CurrencyEntity { Id = 2, Code = "USD" },
                   new CurrencyEntity { Id = 3, Code = "UAH" }
            );

            modelBuilder.Entity<ShippingMethodEntity>()
                .HasData(
                   new ShippingMethodEntity { Id = 1, Name = "Новая почта" },
                   new ShippingMethodEntity { Id = 2, Name = "Укрпочта" },
                   new ShippingMethodEntity { Id = 3, Name = "Самовывоз" }
            );

            modelBuilder.Entity<OrderStatusEntity>()
                .HasData(
                   new OrderStatusEntity { Id = 1, Name = "Готов" },
                   new OrderStatusEntity { Id = 2, Name = "К отправке" },
                   new OrderStatusEntity { Id = 3, Name = "Оплачен полностью" },
                   new OrderStatusEntity { Id = 4, Name = "НОВЫЙ" },
                   new OrderStatusEntity { Id = 5, Name = "Выставлен счёт" },
                   new OrderStatusEntity { Id = 6, Name = "Оплачен частично" },
                   new OrderStatusEntity { Id = 7, Name = "Отправлен" }
            );

            modelBuilder.Entity<ExchangeRateEntity>()
                .HasData(
                   new ExchangeRateEntity { Id = 1, Date = new DateTime(2000, 01, 01), CurrencyId = 3, Value = 1 }
            );
        }
    }
}
