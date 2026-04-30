using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Clients
{
    public class AllClientsDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
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
