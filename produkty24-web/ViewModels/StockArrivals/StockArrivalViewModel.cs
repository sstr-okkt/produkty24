using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.ViewModels.StockArrivals
{
    public class StockArrivalViewModel
    {
        private DateTime date;

        public int Id { get; set; }
        public DateTime Date
        {
            get { return date; }
            set { date = value.ToLocalTime(); }
        }
        public StockItemEntity StockItem { get; set; }
        public float Quantity { get; set; }
    }
}
