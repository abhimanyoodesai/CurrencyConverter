using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.DataAccess;
using CurrencyConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CurrencyRateService> _logger;


        public CurrencyRateService(AppDbContext context, ILogger<CurrencyRateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CurrencyRate?> GetRateAsync(string currencyCode)
        {
            _logger.LogInformation("Fetching rate for {CurrencyCode}", currencyCode);
            return await _context.CurrencyRates.FirstOrDefaultAsync(r => r.CurrencyCode == currencyCode);
        }

        public async Task<List<CurrencyRate>> GetAllRatesAsync()
        {
            _logger.LogInformation("Fetching all currency rates from database");
            return await _context.CurrencyRates.ToListAsync();
        }

        public async Task<decimal> ConvertToDkkAsync(string fromCurrency, decimal amount)
        {
            _logger.LogInformation("Converting {Amount} {FromCurrency} to DKK", amount, fromCurrency);
            var rate = await GetRateAsync(fromCurrency);
            if (rate == null)
            {
                _logger.LogWarning("Rate not found for {FromCurrency}", fromCurrency);
                throw new Exception($"Rate not found for {fromCurrency}");
            }
            var converted = amount * rate.Rate;
            _logger.LogInformation("Converted {Amount} {FromCurrency} to {ConvertedAmount} DKK", amount, fromCurrency, converted);
            return converted;
        }

        public async Task<CurrencyConversion> SaveConversionAsync(string fromCurrency, decimal amount, decimal convertedAmount)
        {
            _logger.LogInformation("Saving conversion {Amount} {FromCurrency} -> {ConvertedAmount} DKK", amount, fromCurrency, convertedAmount);
            var conversion = new CurrencyConversion
            {
                FromCurrency = fromCurrency,
                ToCurrency = "DKK",
                InputAmount = amount,
                ConvertedAmount = convertedAmount,
                ConversionDate = DateTime.UtcNow
            };


            _context.Conversions.Add(conversion);
            await _context.SaveChangesAsync();
            return conversion;
        }

        public async Task<List<CurrencyConversion>> GetConversionsAsync()
        {
            _logger.LogInformation("Fetching all past conversions");
            return await _context.Conversions.ToListAsync();
        }

        public async Task<List<CurrencyConversion>> GetConversionsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? fromCurrency = null)
        {
            _logger.LogInformation("Fetching conversions with filters: FromDate={FromDate}, ToDate={ToDate}, FromCurrency={FromCurrency}", fromDate, toDate, fromCurrency);

            var query = _context.Conversions.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(c => c.ConversionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(c => c.ConversionDate <= toDate.Value);

            if (!string.IsNullOrEmpty(fromCurrency))
                query = query.Where(c => c.FromCurrency == fromCurrency);

            return await query.ToListAsync();
        }
    }
}
