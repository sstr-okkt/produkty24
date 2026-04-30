using System.ComponentModel.DataAnnotations;

namespace Produkty24_Web.ViewModels.ExchangeRates
{
    public class ExchangeRateCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "SelectCurrency")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "PositiveValuesOnly")]
        public float Value { get; set; }
    }
}
