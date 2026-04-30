using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Models.Entities
{
    public class ExchangeRateEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyEntity Currency { get; set; }
        public float Value { get; set; }
    }
}
