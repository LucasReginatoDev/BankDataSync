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
    private readonly JsonStorageService _jsonStorageService;

    public Worker(
        ILogger<Worker> logger,
        BrasilApiService brasilApiService,
        BacenApiService bacenApiService,
        JsonStorageService jsonStorageService)
    {
        _logger = logger;
        _brasilApiService = brasilApiService;
        _bacenApiService = bacenApiService;
        _jsonStorageService = jsonStorageService;
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

            // Lista para armazenar os dados de todas as taxas de juros
            var dadosConsolidados = new List<object>();

            // Passo 2: Iterar sobre os bancos e buscar as taxas de juros de cada banco
            foreach (var banco in bancos)
            {
                _logger.LogInformation("Buscando taxas de juros para o banco: {bancoNome}", banco.Name);

                // Buscar as taxas de juros usando o nome do banco
                var taxas = await _bacenApiService.GetTaxasJurosPorBancoAsync(banco.Name);

                if (taxas == null || taxas.Count == 0)
                {
                    _logger.LogWarning("Nenhuma taxa de juros encontrada para o banco: {bancoNome}", banco.Name);

                    // Adiciona uma entrada indicando que nenhuma taxa foi encontrada
                    dadosConsolidados.Add(new
                    {
                        Banco = banco.Name,
                        Codigo = banco.Code,
                        TaxasJuros = "Nenhuma taxa de juros encontrada"
                    });
                }
                else
                {
                    // Adiciona as taxas de juros e o nome do banco à lista consolidada
                    dadosConsolidados.Add(new
                    {
                        Banco = banco.Name,
                        Codigo = banco.Code,
                        TaxasJuros = taxas
                    });
                    _logger.LogInformation("Dados de taxas de juros adicionados para o banco: {bancoNome}", banco.Name);
                }
            }

            // Salva todos os dados em um único arquivo JSON
            _jsonStorageService.SaveToJson(dadosConsolidados);
            _logger.LogInformation("Dados de todas as taxas de juros salvos em arquivo JSON.");

            // Aguardar 5 minutos antes de repetir o processo
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}