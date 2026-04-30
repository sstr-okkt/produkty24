using System.ComponentModel.DataAnnotations;

namespace Produkty24_Web.ViewModels.Clients
{
    public class ClientCreateViewModel
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "NameStringLenght")]
        public string Name { get; set; }


        [MaxLength(30, ErrorMessage = "MaxLenght30")]
        public string? Nickname { get; set; }


        [RegularExpression(@"^\+?[0-9]{3,20}$", ErrorMessage = "PhoneNumberAllowedCharacters")]
        public string? Phone { get; set; }


        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string? Email { get; set; }


        public int? CountryId { get; set; }


        [MaxLength(30, ErrorMessage = "MaxLenght30")]
        public string? City { get; set; }


        [MaxLength(255, ErrorMessage = "MaxLenght255")]
        public string? Address { get; set; }


        public int? ShippingMethodId { get; set; }


        [RegularExpression(@"^[A-Za-z0-9-]{1,20}$", ErrorMessage = "PostalCodeAllowedCharacters")]
        public string? PostalCode { get; set; }


        [MaxLength(255, ErrorMessage = "MaxLenght255")]
        public string? Notes { get; set; }
    }
}
