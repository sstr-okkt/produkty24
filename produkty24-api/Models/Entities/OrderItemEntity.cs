using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.Entities
{
    public class OrderItemEntity
    {
        public int? Id { get; set; }
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; }
        public int StockItemId { get; set; }
        public StockItemEntity StockItem { get; set; }
        public float Quantity { get; set; }
        public float? Price { get; set; }
        public float? Discount { get; set; }
        public float? Total { get; set; }
        public float? Profit { get; set; }
        public float? Expenses { get; set; }
        public float? ExchangeRate { get; set; }
    }
}
