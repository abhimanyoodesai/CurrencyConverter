using AutoMapper;
using CurrencyConverter.Api.Controllers;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Tests.Controllers
{
    [TestClass]
    public class RatesControllerTests
    {
        private Mock<ICurrencyRateService> _serviceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<RatesController>> _loggerMock;
        private RatesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<ICurrencyRateService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RatesController>>();

            _controller = new RatesController(_serviceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetAllRates_ReturnsOkWithRates()
        {
            // Arrange
            var rates = new List<CurrencyRate> { new CurrencyRate { CurrencyCode = "USD", Rate = 7.5m } };
            var mappedRates = new List<CurrencyRateResponse> { new CurrencyRateResponse { CurrencyCode = "USD", Rate = 7.5m } };

            _serviceMock.Setup(s => s.GetAllRatesAsync()).ReturnsAsync(rates);
            _mapperMock.Setup(m => m.Map<List<CurrencyRateResponse>>(rates)).Returns(mappedRates);

            // Act
            var result = await _controller.GetAllRates();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value as List<CurrencyRateResponse>;
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("USD", value[0].CurrencyCode);
        }

        [TestMethod]
        public async Task GetRate_ReturnsOk_WhenRateExists()
        {
            var rate = new CurrencyRate { CurrencyCode = "USD", Rate = 7.5m };
            var mapped = new CurrencyRateResponse { CurrencyCode = "USD", Rate = 7.5m };

            _serviceMock.Setup(s => s.GetRateAsync("USD")).ReturnsAsync(rate);
            _mapperMock.Setup(m => m.Map<CurrencyRateResponse>(rate)).Returns(mapped);

            var result = await _controller.GetRate("USD");

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value as CurrencyRateResponse;
            Assert.AreEqual("USD", value.CurrencyCode);
        }

        [TestMethod]
        public async Task GetRate_ReturnsNotFound_WhenRateDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetRateAsync("EUR")).ReturnsAsync((CurrencyRate)null);

            var result = await _controller.GetRate("EUR");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Convert_ReturnsOkWithConversionResponse()
        {
            var request = new ConvertRequest { FromCurrency = "USD", Amount = 10 };
            var conversion = new CurrencyConversion
            {
                FromCurrency = "USD",
                ToCurrency = "DKK",
                InputAmount = 10,
                ConvertedAmount = 75
            };
            var response = new ConversionResponse { FromCurrency = "USD", ToCurrency = "DKK", ConvertedAmount = 75 };

            _serviceMock.Setup(s => s.ConvertToDkkAsync("USD", 10)).ReturnsAsync(75);
            _serviceMock.Setup(s => s.SaveConversionAsync("USD", 10, 75)).ReturnsAsync(conversion);
            _mapperMock.Setup(m => m.Map<ConversionResponse>(conversion)).Returns(response);

            var result = await _controller.Convert(request);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value as ConversionResponse;
            Assert.AreEqual(75, value.ConvertedAmount);
        }

        [TestMethod]
        public async Task GetConversions_ReturnsBadRequest_WhenFromDateGreaterThanToDate()
        {
            var fromDate = DateTime.UtcNow;
            var toDate = fromDate.AddDays(-1);

            var result = await _controller.GetConversions(fromDate, toDate, "USD");

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Invalid date range: fromDate must be earlier than or equal to toDate.", badRequest.Value);
        }

        [TestMethod]
        public async Task GetConversions_ReturnsOkWithConversions()
        {
            var conversions = new List<CurrencyConversion>
            {
                new CurrencyConversion { FromCurrency = "USD", ToCurrency = "DKK", InputAmount = 10, ConvertedAmount = 75 }
            };
            var mapped = new List<ConversionResponse>
            {
                new ConversionResponse { FromCurrency = "USD", ToCurrency = "DKK", ConvertedAmount = 75 }
            };

            _serviceMock.Setup(s => s.GetConversionsAsync(null, null, null)).ReturnsAsync(conversions);
            _mapperMock.Setup(m => m.Map<List<ConversionResponse>>(conversions)).Returns(mapped);

            var result = await _controller.GetConversions(null, null, null);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value as List<ConversionResponse>;
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("USD", value[0].FromCurrency);
        }
    }
}
