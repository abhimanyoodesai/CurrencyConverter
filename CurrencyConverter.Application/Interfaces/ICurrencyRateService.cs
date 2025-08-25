using CurrencyConverter.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.Interfaces
{
    public interface ICurrencyRateService
    {
        Task<CurrencyRate?> GetRateAsync(string currencyCode);
        Task<List<CurrencyRate>> GetAllRatesAsync();
        Task<decimal> ConvertToDkkAsync(string fromCurrency, decimal amount);
        Task<CurrencyConversion> SaveConversionAsync(string fromCurrency, decimal amount, decimal convertedAmount);
        Task<List<CurrencyConversion>> GetConversionsAsync();
        Task<List<CurrencyConversion>> GetConversionsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? fromCurrency = null);
    }
}
