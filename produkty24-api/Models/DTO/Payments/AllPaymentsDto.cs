using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Payments
{
    public class AllPaymentsDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ClientEntity Client { get; set; }
        public OrderEntity Order { get; set; }
        public float Amount { get; set; }
        public string? Notes { get; set; }
    }
}
