using System.ComponentModel.DataAnnotations;

namespace Produkty24_Web.ViewModels.Payments
{
    public class PaymentCreateViewModel
    {
        public int ClientId { get; set; }
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "PositiveValuesOnly")]
        public float Amount { get; set; }

        [MaxLength(50, ErrorMessage = "StringLenghtLimit")]
        public string? Notes { get; set; }
    }
}
