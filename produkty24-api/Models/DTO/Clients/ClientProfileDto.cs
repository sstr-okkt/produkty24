using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Clients
{
    public class ClientProfileDto : AllClientsDto
    {
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public int OrdersQuantity { get; set; }
        public float PaymentsTotal { get; set; }
    }
}
