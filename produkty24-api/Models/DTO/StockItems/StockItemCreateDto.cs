using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.StockItems
{
    public class StockItemCreateDto
    {
        public string Name { get; set; }
        public int? ManufacturerId { get; set; }
        public string? Description { get; set; }
        public int CurrencyId { get; set; }
        public float PurchasePrice { get; set; }
        public float RetailPrice { get; set; }
    }
}
