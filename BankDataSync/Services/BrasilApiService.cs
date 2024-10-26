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

        // Lista com os códigos dos 20 bancos mais relevantes
        private readonly List<int> _bancosRelevantes = new List<int>
        {
            1,    // Banco do Brasil
            237,  // Bradesco
            341,  // Itaú Unibanco
            104,  // Caixa Econômica Federal
            33,   // Santander Brasil
            745,  // Citibank
            623,  // Banco PAN
            422,  // Safra
            208,  // Banco BTG Pactual
            336,  // Banco C6
            70,   // BRB - Banco de Brasília
            212,  // Banco Original
            260,  // Nubank
            290,  // PagSeguro
            323,  // Mercado Pago
            77,   // Inter
            756,  // Sicoob
            85,   // Ailos
            633,  // Banco Rendimento
            655  // Banco Votorantim (BV)
        };

        public BrasilApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["BrasilApiUrl"];
        }

        /// <summary>
        /// Busca a lista de bancos na Brasil API, filtrando para incluir apenas os bancos mais relevantes.
        /// </summary>
        /// <returns>Lista filtrada com os bancos mais relevantes</returns>
        
        public async Task<List<Banco>> GetBancosAsync()
        {
            // Realiza a requisição para obter todos os bancos
            var response = await _httpClient.GetFromJsonAsync<List<Banco>>(_baseUrl);

            // Filtra apenas os bancos que possuem código dentro da lista de bancos relevantes
            var bancosRelevantes = response?
                .Where(banco => banco.Code.HasValue && _bancosRelevantes.Contains(banco.Code.Value))
                .ToList();

            // Retorna a lista de bancos relevantes ou uma lista vazia se nenhum banco for encontrado
            return bancosRelevantes ?? new List<Banco>();
        }
    }

    /// <summary>
    /// Classe que representa um banco, com apenas os campos 'Name' e 'Code' mapeados da resposta JSON da Brasil API.
    /// </summary>
    
    public class Banco
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }
    }
}
