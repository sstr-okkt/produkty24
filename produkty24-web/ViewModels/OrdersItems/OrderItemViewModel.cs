using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.ViewModels.OrdersItems
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public StockItemEntity StockItem { get; set; }
        public OrderEntity Order { get; set; }
        public float Quantity { get; set; }
    }
}
