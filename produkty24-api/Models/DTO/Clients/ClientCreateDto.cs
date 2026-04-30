using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Clients
{
    public class ClientCreateDto
    {
        public string Name { get; set; }
        public string? Nickname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? CountryId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public int? ShippingMethodId { get; set; }
        public string? PostalCode { get; set; }
        public string? Notes { get; set; }
    }
}
