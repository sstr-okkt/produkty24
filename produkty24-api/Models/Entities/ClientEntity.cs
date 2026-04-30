using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Produkty24_API.Models.Entities
{
    public class ClientEntity
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string? Nickname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? Notes { get; set; }

        public int? ShippingMethodId { get; set; }
        public ShippingMethodEntity? ShippingMethod { get; set; }
        public int? CountryId { get; set; }
        public CountryEntity? Country { get; set; }
    }
}
