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
    public class BrasilApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        // Lista com os códigos de 10 bancos relevantes
        private readonly List<int> _bancosRelevantes = new List<int>
    {
        1,    // Banco do Brasil
        237,  // Bradesco
        341,  // Itaú Unibanco
        104,  // Caixa Econômica Federal
        33,   // Santander Brasil
        77,   // Inter
        212,  // Banco Original
        260,  // Nubank
        70,   // BRB - Banco de Brasília
        290   // PagSeguro
    };

        public BrasilApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BrasilApiUrl"];
        }

        public async Task<List<Banco>> GetBancosAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Banco>>(_baseUrl);
            return response?
                .Where(banco => banco.Code.HasValue && _bancosRelevantes.Contains(banco.Code.Value))
                .ToList() ?? new List<Banco>();
        }
    }

    public class Banco
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }
    }
}
