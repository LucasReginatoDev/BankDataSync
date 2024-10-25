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
        public async Task<List<TaxaJurosDiaria>?> GetTaxasJurosPorBancoAsync(string bancoCodigo)
        {
            var url = $"https://olinda.bcb.gov.br/olinda/servico/taxaJuros/versao/v2/odata/TaxasJurosDiariaPorInicioPeriodo?$filter=InstituicaoFinanceira eq '{bancoCodigo}'&$top=50&$format=json";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);

            // Desserializar para a classe RespostaTaxaJurosDiaria
            var resultado = JsonConvert.DeserializeObject<RespostaTaxaJurosDiaria>(response);

            // Retornar a lista de taxas de juros do campo `value`
            return resultado?.Value;
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
            public List<TaxaJurosDiaria> ?Value { get; set; }
        }
    }
}