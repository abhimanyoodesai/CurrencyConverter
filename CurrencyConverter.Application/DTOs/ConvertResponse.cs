    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.DTOs
{
    public class ConvertResponse
    {
        public string? FromCurrency { get; set; }
        public string ToCurrency { get; set; } = "DKK";
        public decimal ConvertedAmount { get; set; }
    }
}
