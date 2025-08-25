using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.DataAccess.Entities
{
    public class CurrencyConversion
    {
        public int Id { get; set; }
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public decimal InputAmount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public DateTime ConversionDate { get; set; }
    }
}
