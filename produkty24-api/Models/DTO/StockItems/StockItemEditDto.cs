using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.StockItems
{
    public class StockItemEditDto : StockItemCreateDto
    {
        public int Id { get; set; }
    }
}
