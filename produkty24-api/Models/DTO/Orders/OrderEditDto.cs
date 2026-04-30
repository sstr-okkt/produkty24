using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.Orders
{
    public class OrderEditDto : OrderCreateDto
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
    }
}
