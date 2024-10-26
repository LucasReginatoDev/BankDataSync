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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _brasilApiService = brasilApiService ?? throw new ArgumentNullException(nameof(brasilApiService));
        _bacenApiService = bacenApiService ?? throw new ArgumentNullException(nameof(bacenApiService));
        _jsonStorageService = jsonStorageService ?? throw new ArgumentNullException(nameof(jsonStorageService));

        _logger.LogInformation("Worker constructor initialized");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker rodando em: {time}", DateTimeOffset.Now);

        try
        {
            // Loop principal enquanto o serviço não estiver sendo cancelado
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando processo de busca de dados.");

                // Passo 1: Buscar bancos na Brasil API
                var bancos = await _brasilApiService.GetBancosAsync();
                if (bancos == null || bancos.Count == 0)
                {
                    _logger.LogWarning("Nenhum banco encontrado na Brasil API.");
                }
                else
                {
                    var dadosConsolidados = new List<object>();

                    // Passo 2: Iterar sobre os bancos e buscar as taxas de juros de cada banco
                    foreach (var banco in bancos)
                    {
                        try
                        {
                            _logger.LogInformation("Buscando taxas de juros para o banco: {bancoNome}", banco.Name);

                            // Buscar as taxas de juros usando o nome do banco
                            var taxas = await _bacenApiService.GetTaxasJurosPorBancoAsync(banco.Name);

                            if (taxas == null || taxas.Count == 0)
                            {
                                _logger.LogWarning("Nenhuma taxa de juros encontrada para o banco: {bancoNome}", banco.Name);
                                dadosConsolidados.Add(new { Banco = banco.Name, Codigo = banco.Code, TaxasJuros = "Nenhuma taxa de juros encontrada" });
                            }
                            else
                            {
                                dadosConsolidados.Add(new { Banco = banco.Name, Codigo = banco.Code, TaxasJuros = taxas });
                                _logger.LogInformation("Dados de taxas de juros adicionados para o banco: {bancoNome}", banco.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao buscar taxas de juros para o banco: {bancoNome}", banco.Name);
                            // Registra erro no JSON para o banco atual
                            dadosConsolidados.Add(new { Banco = banco.Name, Codigo = banco.Code, Erro = "Falha na obtenção de dados: " + ex.Message });
                        }
                    }

                    // Salva todos os dados (incluindo erros) em um único arquivo JSON
                    _jsonStorageService.SaveToJson(dadosConsolidados);
                    _logger.LogInformation("Dados de todas as taxas de juros salvos em arquivo JSON.");
                }

                // Intervalo entre as execuções
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            // Captura qualquer exceção inesperada durante a execução
            _logger.LogError(ex, "Erro inesperado durante a execução do Worker.");
            throw; // Relança a exceção para ser registrada corretamente no Event Viewer
        }
    }
}
