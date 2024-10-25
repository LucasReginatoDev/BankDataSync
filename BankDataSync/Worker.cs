using BankDataSync.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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
            _logger.LogInformation("Worker rodando em: {time}", DateTimeOffset.Now);

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

                // Buscar as taxas de juros usando o nome do banco (em vez de código)
                var taxas = await _bacenApiService.GetTaxasJurosPorBancoAsync(banco.Name);

                if (taxas == null || taxas.Count == 0)
                {
                    _logger.LogWarning("Nenhuma taxa de juros encontrada para o banco: {bancoNome} (Código: {bancoCodigo})", banco.Name, banco.Code);
                }
                else
                {
                    _logger.LogInformation("Número de taxas de juros encontradas para o banco {bancoNome}: {count}", banco.Name, taxas.Count);

                    // Log de detalhes de cada taxa de juros (opcional)
                    foreach (var taxa in taxas)
                    {
                        _logger.LogInformation(
                            "Banco: {banco}, Modalidade: {modalidade}, Taxa ao Mês: {taxaMes}%, Taxa ao Ano: {taxaAno}%",
                            taxa.InstituicaoFinanceira, taxa.Modalidade, taxa.TaxaJurosAoMes, taxa.TaxaJurosAoAno);
                    }
                }
            }

            // Intervalo de execução definido para 5 minutos
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
