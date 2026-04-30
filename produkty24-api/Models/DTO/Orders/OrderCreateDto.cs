using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Orders
{
    public class OrderCreateDto
    {
        public int ClientId { get; set; }
        public string? Notes { get; set; }
    }
}
