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

            // Passo 1: Buscar bancos na Brasil API
            var bancos = await _brasilApiService.GetBancosAsync();
            if (bancos.Count == 0)
            {
                _logger.LogWarning("Nenhum banco encontrado na Brasil API.");
                return;
            }

            _logger.LogInformation("Número de bancos encontrados: {count}", bancos.Count);

            // Passo 2: Iterar sobre os bancos e buscar as taxas de juros de cada banco
            foreach (var banco in bancos)
            {
                _logger.LogInformation("Buscando taxas de juros para o banco: {bancoNome} (Código: {bancoCodigo})", banco.Name, banco.Code);

                // Buscar as taxas de juros para o banco atual
                var taxas = await _bacenApiService.GetTaxasJurosPorBancoAsync(banco.Code.ToString());

                if (taxas.Count == 0)
                {
                    _logger.LogWarning("Nenhuma taxa de juros encontrada para o banco: {bancoNome} (Código: {bancoCodigo})", banco.Name, banco.Code);
                }
                else
                {
                    _logger.LogInformation("Número de taxas de juros encontradas para o banco {bancoNome}: {count}", banco.Name, taxas.Count);

                    // Aqui você pode logar mais informações ou processar as taxas conforme necessário.
                }
            }

            // Definir intervalo entre execuções (1 minuto no exemplo)
            await Task.Delay(60000, stoppingToken);
        }
    }
}
