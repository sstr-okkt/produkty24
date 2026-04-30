using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.ViewModels.Payments
{
    public class PaymentViewModel
    {
        private DateTime date;

        public int Id { get; set; }
        public DateTime Date {
            get { return date.ToLocalTime(); }
            set { date = value; }
        }
        public ClientEntity Client { get; set; }
        public OrderEntity Order { get; set; }
        public float Amount { get; set; }
        public string? Notes { get; set; }
    }
}
