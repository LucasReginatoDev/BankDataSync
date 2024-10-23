using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BankDataSync.Services
{
    public class BacenApiService
    {
        private readonly HttpClient _httpClient;

        public BacenApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // URL da API do Banco Central
        private const string BacenApiUrl = "https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativasMercadoSelic?$top=100&$format=json&$select=Indicador,Media,Mediana,DesvioPadrao,Minimo,Maximo,baseCalculo";

        public async Task<List<IndicadorDto>> GetBacenDataAsync()
        {
            // Faz a requisição GET para a API do Banco Central
            var response = await _httpClient.GetAsync(BacenApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Converte a resposta JSON para uma lista de Indicadores
                var data = JsonConvert.DeserializeObject<List<IndicadorDto>>(content);
                return data;
            }

            // Caso ocorra um erro, você pode fazer o log do erro e lançar uma exceção
            throw new HttpRequestException("Falha ao obter dados da API do Banco Central");
        }
    }

    // Definição do DTO que representa os dados da API
    public class IndicadorDto
    {
        public string? Indicador { get; set; } 
        public double? Media { get; set; }
        public double? Mediana { get; set; }
        public double? DesvioPadrao { get; set; }
        public double? Minimo { get; set; }
        public double? Maximo { get; set; }
        public int? BaseCalculo { get; set; }
    }
}