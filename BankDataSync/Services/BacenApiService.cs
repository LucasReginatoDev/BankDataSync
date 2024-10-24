using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;

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
        public async Task<List<TaxaJuros>> GetTaxasJurosPorBancoAsync(string bancoCodigo)
        {
            try
            {
                string url = $"https://olinda.bcb.gov.br/olinda/servico/taxaJuros/versao/v2/odata/TaxasJurosDiariaPorInicioPeriodo?$filter=InstituicaoFinanceira eq '{bancoCodigo}'&$top=50&$format=json";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var taxas = JsonConvert.DeserializeObject<List<TaxaJuros>>(content);
                return taxas ?? new List<TaxaJuros>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Erro ao buscar taxas de juros para o banco {bancoCodigo}: {ex.Message}");
                return new List<TaxaJuros>();
            }
        }

        public class TaxaJuros
        {
            public string InstituicaoFinanceira { get; set; }
            public DateTime DataReferencia { get; set; }
            public double TaxaJurosAoAno { get; set; }
            public double TaxaJurosAoMes { get; set; }
            public double Modalidade { get; set; }
        }
    }
}