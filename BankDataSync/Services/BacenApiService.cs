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
        private readonly string _baseUrl;

        public BacenApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BacenApiUrl"];
        }

        public async Task<List<TaxaJuros>> GetTaxasJurosAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<TaxaJuros>>(_baseUrl);
            return response ?? new List<TaxaJuros>();
        }
    }

    public class TaxaJuros
    {
        public string Data { get; set; }
        public double Valor { get; set; }
    }
}