using BankDataSync.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly BrasilApiService _brasilApiService;
    private readonly BacenApiService _bacenApiService;

    public Worker(ILogger<Worker> logger, BrasilApiService brasilApiService, BacenApiService bacenApiService)
    {
        _logger = logger;
        _brasilApiService = brasilApiService;
        _bacenApiService = bacenApiService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // Consome a Brasil API
            var bancos = await _brasilApiService.GetBancosAsync();
            _logger.LogInformation($"Número de bancos encontrados: {bancos.Count}");

            // Consome a API do Banco Central
            var taxasJuros = await _bacenApiService.GetTaxasJurosAsync();
            _logger.LogInformation($"Número de taxas de juros encontradas: {taxasJuros.Count}");

            await Task.Delay(60000, stoppingToken); // Espera 1 minuto entre cada execução
        }
    }
}
