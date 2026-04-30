using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.ViewModels.Clients
{
    public class ClientViewModel
    {
        private DateTime date;

        public int Id { get; set; }
        public DateTime Date {
            get { return date.ToLocalTime(); }
            set { date = value; }
        }
        public string Name { get; set; }
        public string? Nickname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public CountryEntity? Country { get; set; }
        public string? City { get; set; }
        public ShippingMethodEntity? ShippingMethod { get; set; }
        public string? PostalCode { get; set; }
    }
}
