using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Payments
{
    public class PaymentCreateDto
    {
        public int ClientId { get; set; }
        public int OrderId { get; set; }
        public float Amount { get; set; }
        public string? Notes { get; set; }
    }
}
