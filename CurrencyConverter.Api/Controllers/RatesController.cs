using AutoMapper;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatesController : ControllerBase
    {
        private readonly ICurrencyRateService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<RatesController> _logger;
        public RatesController(ICurrencyRateService service, IMapper mapper, ILogger<RatesController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRates()
        {
            _logger.LogInformation("API Call: GET /api/rates at {Time}", DateTime.UtcNow);
            var rates = await _service.GetAllRatesAsync();
            var response = _mapper.Map<List<CurrencyRateResponse>>(rates);
            return Ok(response);
        }

        [HttpGet("{currencyCode}")]
        public async Task<IActionResult> GetRate(string currencyCode)
        {
            _logger.LogInformation("API Call: GET /api/rates/{CurrencyCode} at {Time}", currencyCode, DateTime.UtcNow);
            var rate = await _service.GetRateAsync(currencyCode);
            if (rate == null)
            {
                _logger.LogWarning("Rate not found for {CurrencyCode}", currencyCode);
                return NotFound();
            }

            return Ok(_mapper.Map<CurrencyRateResponse>(rate));
        }

        [HttpPost("convert")]
        public async Task<IActionResult> Convert([FromBody] ConvertRequest request)
        {
            _logger.LogInformation("API Call: POST /api/rates/convert with {Amount} {Currency} at {Time}", request.Amount, request.FromCurrency, DateTime.UtcNow);
            var result = await _service.ConvertToDkkAsync(request.FromCurrency, request.Amount);
            var conversion = await _service.SaveConversionAsync(request.FromCurrency, request.Amount, result);


            var response = _mapper.Map<ConversionResponse>(conversion);
            return Ok(response);
        }

        [HttpGet("conversions")]
        public async Task<IActionResult> GetConversions([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] string? fromCurrency)
        {
            _logger.LogInformation("API Call: GET /api/rates/conversions with filters at {Time}", DateTime.UtcNow);

            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                _logger.LogWarning("Invalid date range: FromDate {FromDate} is later than ToDate {ToDate}", fromDate, toDate);
                return BadRequest("Invalid date range: fromDate must be earlier than or equal to toDate.");
            }

            var conversions = await _service.GetConversionsAsync(fromDate, toDate, fromCurrency);
            var response = _mapper.Map<List<ConversionResponse>>(conversions);
            return Ok(response);
        }
    }
}
