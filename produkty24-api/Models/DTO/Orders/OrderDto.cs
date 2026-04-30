using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ClientEntity Client { get; set; }
        public OrderStatusEntity Status { get; set; }
        public string Notes { get; set; }
    }
}
