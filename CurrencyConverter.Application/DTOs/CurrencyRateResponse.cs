using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.DTOs
{
    public class CurrencyRateResponse
    {
        public string? CurrencyCode { get; set; }
        public decimal Rate { get; set; }
    }
}
