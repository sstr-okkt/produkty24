using System.ComponentModel.DataAnnotations;

namespace Produkty24_Web.ViewModels.StockItems
{
    public class StockItemCreateViewModel
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "NameStringLenght")]
        public string Name { get; set; }
        
        public int? ManufacturerId { get; set; }

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SelectCurrency")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "PositiveValuesOnly")]
        public float PurchasePrice { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "PositiveValuesOnly")]
        public float RetailPrice { get; set; }
    }
}
