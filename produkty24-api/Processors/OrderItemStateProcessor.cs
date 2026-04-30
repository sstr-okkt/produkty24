using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Processors
{
    public class OrderItemStateProcessor
    {
        private readonly IEnumerable<ExchangeRateEntity>? exchangeRates;

        public OrderItemStateProcessor() { }
        public OrderItemStateProcessor(IEnumerable<ExchangeRateEntity> exchangeRates)
        {
            this.exchangeRates = exchangeRates;
        }

        public void Calculate(OrderItemEntity orderItem)
        {
            if (exchangeRates != null) {
                foreach (var exchangeRate in exchangeRates) {
                    if (orderItem.StockItem.CurrencyId == exchangeRate.CurrencyId)
                        orderItem.ExchangeRate = exchangeRate.Value;
                }
            }            

            orderItem.Price = orderItem.StockItem.RetailPrice * orderItem.ExchangeRate;

            var discount = orderItem.Quantity * orderItem.Price * (orderItem.Discount * 0.01f);
            orderItem.Total = orderItem.Quantity * orderItem.Price - discount;

            orderItem.Expenses = orderItem.Quantity * (orderItem.StockItem.PurchasePrice * orderItem.ExchangeRate);
            orderItem.Profit = orderItem.Total - orderItem.Expenses;
        }
    }
}
