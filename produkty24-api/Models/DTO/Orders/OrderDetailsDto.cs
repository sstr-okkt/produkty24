using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Orders
{
    public class OrderDetailsDto : OrderEntity
    {
        public IEnumerable<OrderItemEntity> Items { get; set; }
        public float Total { get; set; }
        public float PaymentsTotal { get; set; }
        public float Debt { get; set; }
    }
}
