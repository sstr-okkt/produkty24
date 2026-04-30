using Produkty24_Web.Models.Entities;

namespace Produkty24_Web.ViewModels.ExchangeRates
{
    public class ExchangeRateViewModel
    {
        private DateTime date;

        public int Id { get; set; }
        public DateTime Date
        {
            get { return date; }
            set { date = value.ToLocalTime(); }
        }
        public CurrencyEntity Currency { get; set; }
        public double Value { get; set; }
    }
}
