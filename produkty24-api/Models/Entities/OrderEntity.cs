using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public ClientEntity Client { get; set; }
        public int StatusId { get; set; }
        public OrderStatusEntity Status { get; set; }
        public string? Notes { get; set; }
    }
}
