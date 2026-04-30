using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.ViewModels.Orders
{
    public class OrderViewModel
    {
        private DateTime date;

        public int Id { get; set; }
        public DateTime Date
        {
            get { return date; }
            set { date = value.ToLocalTime(); }
        }
        public ClientEntity Client { get; set; }
        public OrderStatusEntity Status { get; set; }
        public string Notes { get; set; }
    }
}
