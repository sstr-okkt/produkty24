using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.Entities
{
    public class StockArrivalEntity
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public int StockItemId { get; set; }
        public StockItemEntity StockItem { get; set; }
        public float Quantity { get; set; }
    }
}
