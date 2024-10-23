using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BankDataSync.Services
{
    public class BrasilApiService
    {
        private readonly HttpClient _httpClient;

        public BrasilApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // URL da API da Brasil API
        private const string BrasilApiUrl = "https://brasilapi.com.br/api/banks/v1";

        public async Task<List<BancoDto>> GetBancosAsync()
        {
            // Faz a requisição GET para a Brasil API
            var response = await _httpClient.GetAsync(BrasilApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Converte a resposta JSON para uma lista de Bancos
                var data = JsonConvert.DeserializeObject<List<BancoDto>>(content);
                return data;
            }

            // Caso ocorra um erro, você pode fazer o log do erro e lançar uma exceção
            throw new HttpRequestException("Falha ao obter dados da Brasil API");
        }
    }

    // Definição do DTO que representa os dados da API
    public class BancoDto
    {
        public string ISPB { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
    }
}
