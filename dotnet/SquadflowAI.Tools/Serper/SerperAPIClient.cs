using SquadflowAI.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.Serper
{
    public class SerperAPIClient : ISerperAPIClient
    {
        private readonly HttpClient _httpClient;

        public SerperAPIClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<string> MakeRequestAsync(string query)
        {
            var data = new
            {
                q = query,
                gl = "gb"
            };

            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            return await response.Content.ReadAsStringAsync();
        }

    }
}
