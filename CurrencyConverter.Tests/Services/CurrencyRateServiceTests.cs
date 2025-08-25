using CurrencyConverter.Application.Services;
using CurrencyConverter.DataAccess;
using CurrencyConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyConverter.Tests.Services
{
    [TestClass]
    public class CurrencyRateServiceTests
    {
        private AppDbContext? _context;
        private CurrencyRateService? _service;
        private Mock<ILogger<CurrencyRateService>>? _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // new DB each test
                .Options;

            _context = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<CurrencyRateService>>();
            _service = new CurrencyRateService(_context, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetRateAsync_ReturnsRate_WhenCurrencyExists()
        {
            // Arrange
            _context.CurrencyRates.Add(new CurrencyRate { CurrencyCode = "USD", Rate = 7.5m });
            await _context.SaveChangesAsync();

            // Act
            var rate = await _service.GetRateAsync("USD");

            // Assert
            Assert.IsNotNull(rate);
            Assert.AreEqual("USD", rate.CurrencyCode);
            Assert.AreEqual(7.5m, rate.Rate);
        }

        [TestMethod]
        public async Task GetRateAsync_ReturnsNull_WhenCurrencyDoesNotExist()
        {
            var rate = await _service.GetRateAsync("EUR");
            Assert.IsNull(rate);
        }

        [TestMethod]
        public async Task GetAllRatesAsync_ReturnsAllRates()
        {
            _context.CurrencyRates.AddRange(
                new CurrencyRate { CurrencyCode = "USD", Rate = 7.5m },
                new CurrencyRate { CurrencyCode = "EUR", Rate = 8.0m }
            );
            await _context.SaveChangesAsync();

            var rates = await _service.GetAllRatesAsync();

            Assert.AreEqual(2, rates.Count);
        }

        [TestMethod]
        public async Task ConvertToDkkAsync_ReturnsConvertedAmount_WhenRateExists()
        {
            _context.CurrencyRates.Add(new CurrencyRate { CurrencyCode = "USD", Rate = 7.5m });
            await _context.SaveChangesAsync();

            var result = await _service.ConvertToDkkAsync("USD", 10);

            Assert.AreEqual(75.0m, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ConvertToDkkAsync_ThrowsException_WhenRateNotFound()
        {
            await _service.ConvertToDkkAsync("INR", 100);
        }

        [TestMethod]
        public async Task SaveConversionAsync_SavesAndReturnsConversion()
        {
            var conversion = await _service.SaveConversionAsync("USD", 10, 75);

            var saved = await _context.Conversions.FirstOrDefaultAsync();

            Assert.IsNotNull(saved);
            Assert.AreEqual("USD", saved.FromCurrency);
            Assert.AreEqual(75, saved.ConvertedAmount);
            Assert.AreEqual(conversion.Id, saved.Id);
        }

        [TestMethod]
        public async Task GetConversionsAsync_ReturnsAllConversions()
        {
            _context.Conversions.AddRange(
                new CurrencyConversion { FromCurrency = "USD", ToCurrency = "DKK", InputAmount = 10, ConvertedAmount = 75, ConversionDate = DateTime.UtcNow },
                new CurrencyConversion { FromCurrency = "EUR", ToCurrency = "DKK", InputAmount = 20, ConvertedAmount = 160, ConversionDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var conversions = await _service.GetConversionsAsync();

            Assert.AreEqual(2, conversions.Count);
        }

        [TestMethod]
        public async Task GetConversionsAsync_WithFilters_ReturnsFilteredResults()
        {
            var now = DateTime.UtcNow;
            _context.Conversions.AddRange(
                new CurrencyConversion { FromCurrency = "USD", ToCurrency = "DKK", InputAmount = 10, ConvertedAmount = 75, ConversionDate = now.AddDays(-2) },
                new CurrencyConversion { FromCurrency = "USD", ToCurrency = "DKK", InputAmount = 20, ConvertedAmount = 150, ConversionDate = now },
                new CurrencyConversion { FromCurrency = "EUR", ToCurrency = "DKK", InputAmount = 30, ConvertedAmount = 225, ConversionDate = now }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetConversionsAsync(fromDate: now.AddDays(-1), fromCurrency: "USD");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(20, result[0].InputAmount);
        }
    }
}
