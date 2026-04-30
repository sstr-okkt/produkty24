using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.OrdersItems
{
    public class OrderItemEditDto : OrderItemCreateDto
    {
        public int Id { get; set; }
    }
}
