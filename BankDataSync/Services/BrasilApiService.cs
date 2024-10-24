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
    public class BrasilApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public BrasilApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BrasilApiUrl"];
        }

        public async Task<List<Banco>> GetBancosAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Banco>>(_baseUrl);
            return response ?? new List<Banco>();
        }
    }

    public class Banco
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
