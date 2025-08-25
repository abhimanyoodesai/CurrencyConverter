using CurrencyConverter.DataAccess;
using CurrencyConverter.DataAccess.Entities;
using System.Xml.Linq;

namespace CurrencyConverter.Api.BackgroundJobs
{
    public class CurrencyFetchBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<CurrencyFetchBackgroundService> _logger;
        private readonly HttpClient _httpClient;

        private const string ApiUrl = "https://www.nationalbanken.dk/api/currencyratesxml?lang=en";

        public CurrencyFetchBackgroundService(IServiceProvider services, ILogger<CurrencyFetchBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
            _httpClient = new HttpClient();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    _logger.LogInformation("Fetching currency rates from Nationalbanken API at {Time}", DateTime.UtcNow);
                    var xmlString = await _httpClient.GetStringAsync(ApiUrl);
                    var xdoc = XDocument.Parse(xmlString);

                    var rates = xdoc.Descendants("currency")
                    .Select(c => new CurrencyRate
                    {
                        CurrencyCode = c.Attribute("code")?.Value ?? string.Empty,
                        Rate = (decimal.Parse(c.Attribute("rate")?.Value.Replace(",", ".") ?? "0", System.Globalization.CultureInfo.InvariantCulture)) /100,
                        DateFetched = DateTime.UtcNow
                    })
                    .Where(r => !string.IsNullOrEmpty(r.CurrencyCode))
                    .ToList();

                    if (rates.Any())
                    {
                        _logger.LogInformation("Updating {Count} currency rates in database", rates.Count);
                        db.CurrencyRates.RemoveRange(db.CurrencyRates);
                        await db.SaveChangesAsync();


                        await db.CurrencyRates.AddRangeAsync(rates);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogWarning("No rates were parsed from API response");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching or saving currency rates");
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
