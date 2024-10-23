using BankDataSync.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly BacenApiService _bacenApiService;
    private readonly BrasilApiService _brasilApiService;

    public Worker(ILogger<Worker> logger, BacenApiService bacenApiService, BrasilApiService brasilApiService)
    {
        _logger = logger;
        _bacenApiService = bacenApiService;
        _brasilApiService = brasilApiService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // Consumir a API do Banco Central
            var bacenData = await _bacenApiService.GetBacenDataAsync();
            _logger.LogInformation("Dados da API Bacen obtidos com sucesso.");

            // Consumir a Brasil API
            var bancosData = await _brasilApiService.GetBancosAsync();
            _logger.LogInformation("Dados da Brasil API obtidos com sucesso.");

            // Processamento dos dados pode ser feito aqui

            await Task.Delay(300000, stoppingToken); // Executa a cada 5 minutos
        }
    }
}
