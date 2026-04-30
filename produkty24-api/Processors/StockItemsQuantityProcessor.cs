using Produkty24_API.Db;
using Produkty24_API.Models.DTO.StockItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Processors
{
    public class StockItemsQuantityProcessor
    {
        private readonly DataContext dataContext;

        public StockItemsQuantityProcessor(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void GetQuantity(IEnumerable<AllStockItemsDto> stockItems)
        {
            foreach (var stockItem in stockItems) {
                var inOrders = dataContext.OrdersItems.Where(i => i.StockItemId == stockItem.Id)
                    .Sum(i => i.Quantity);
                var inArrivals = dataContext.StockArrivals.Where(i => i.StockItemId == stockItem.Id)
                    .Sum(i => i.Quantity);
                stockItem.Quantity = inArrivals - inOrders;
            }
        }
    }
}
