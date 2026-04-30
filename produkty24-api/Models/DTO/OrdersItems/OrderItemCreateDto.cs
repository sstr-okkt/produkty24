using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.OrdersItems
{
    public class OrderItemCreateDto
    {
        public int OrderId { get; set; }
        public int StockItemId { get; set; }
        public float Quantity { get; set; }
        public float Discount { get; set; }
    }
}
