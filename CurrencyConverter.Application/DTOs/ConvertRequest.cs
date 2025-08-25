using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.DTOs
{
    public class ConvertRequest
    {
        public string? FromCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
