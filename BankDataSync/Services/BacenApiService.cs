using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

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
}