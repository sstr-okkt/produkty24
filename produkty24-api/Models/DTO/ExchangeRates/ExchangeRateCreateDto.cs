using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.DTO.ExchangeRates
{
    public class ExchangeRateCreateDto
    {
        public int CurrencyId { get; set; }
        public float Value { get; set; }
    }
}
