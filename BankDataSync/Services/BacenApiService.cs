using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace BankDataSync.Services
{
    public class BacenApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BacenApiService> _logger;

        public BacenApiService(HttpClient httpClient, ILogger<BacenApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Método para buscar taxas de juros por banco
        public async Task<List<TaxaJurosDiaria>> GetTaxasJurosPorBancoAsync(string bancoNome)
        {
            var url = $"https://olinda.bcb.gov.br/olinda/servico/taxaJuros/versao/v2/odata/TaxasJurosDiariaPorInicioPeriodo?$filter=InstituicaoFinanceira eq '{bancoNome}'&$top=50&$format=json";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var resultado = JsonConvert.DeserializeObject<RespostaTaxaJurosDiaria>(response);
                return resultado?.Value ?? new List<TaxaJurosDiaria>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao buscar taxas de juros da API do Bacen.");
                return new List<TaxaJurosDiaria>();
            }
        }        
    }

    public class TaxaJurosDiaria
    {
        [JsonPropertyName("InicioPeriodo")]
        public string? InicioPeriodo { get; set; }

        [JsonPropertyName("FimPeriodo")]
        public string? FimPeriodo { get; set; }

        [JsonPropertyName("Segmento")]
        public string? Segmento { get; set; }

        [JsonPropertyName("Modalidade")]
        public string? Modalidade { get; set; }

        [JsonPropertyName("Posicao")]
        public int? Posicao { get; set; }

        [JsonPropertyName("InstituicaoFinanceira")]
        public string? InstituicaoFinanceira { get; set; }

        [JsonPropertyName("TaxaJurosAoMes")]
        public double? TaxaJurosAoMes { get; set; }

        [JsonPropertyName("TaxaJurosAoAno")]
        public double? TaxaJurosAoAno { get; set; }
    }

    public class RespostaTaxaJurosDiaria
    {
        [JsonProperty("value")]
        public List<TaxaJurosDiaria>? Value { get; set; }
    }

    public class JsonStorageService
    {
        private readonly string _outputDirectory = "DataStorage";
        private readonly string _fileName = "TaxasJuros.json"; // Nome fixo para o arquivo JSON

        public JsonStorageService()
        {
            // Cria o diretório para armazenar o arquivo JSON, caso não exista
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        // Método para salvar todos os dados em um único arquivo JSON, substituindo o arquivo anterior
        public void SaveToJson(List<object> dadosConsolidados)
        {
            string filePath = Path.Combine(_outputDirectory, _fileName);

            try
            {
                // Verifica e deleta o arquivo anterior, se existir
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Cria um objeto para armazenar a data da consulta e os dados consolidados
                var dadosArquivo = new
                {
                    DataConsulta = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Bancos = dadosConsolidados
                };

                // Serializa os dados incluindo a data e hora da consulta
                string jsonData = JsonConvert.SerializeObject(dadosArquivo, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine($"Dados salvos em JSON no caminho: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar dados em JSON: {ex.Message}");
            }
        }
    }
}