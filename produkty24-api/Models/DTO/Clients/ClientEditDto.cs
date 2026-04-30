using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Clients
{
    public class ClientEditDto : ClientCreateDto
    {
        public int Id { get; set; }
    }
}
