using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.DataAccess.Entities
{
    public class CurrencyRate
    {
        public string? CurrencyCode { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateFetched { get; set; }
    }
}
