using BankDataSync.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

            // Passo 1: Buscar apenas os bancos relevantes na Brasil API
            var bancos = await _brasilApiService.GetBancosRelevantesAsync();
            if (bancos.Count == 0)
            {
                _logger.LogWarning("Nenhum banco relevante encontrado na Brasil API.");
                return;
            }

            _logger.LogInformation("Número de bancos relevantes encontrados: {count}", bancos.Count);

            // Passo 2: Iterar sobre os bancos relevantes e buscar as taxas de juros de cada um, agora usando o nome do banco
            foreach (Banco banco in bancos)
            {
                _logger.LogInformation("Buscando taxas de juros para o banco: {bancoNome}", banco.Name);

                // Usar o nome do banco para buscar as taxas de juros
                List<TaxaJurosDiaria>? taxas = await _bacenApiService.GetTaxasJurosPorBancoAsync(banco.Name);

                if (taxas == null || taxas.Count == 0)
                {
                    _logger.LogWarning("Nenhuma taxa de juros encontrada para o banco: {bancoNome}", banco.Name);
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
