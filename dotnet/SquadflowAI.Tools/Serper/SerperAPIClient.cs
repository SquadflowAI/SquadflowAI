using SquadflowAI.Contracts;
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
    public class SerperAPIClient : ITool //ISerperAPIClient
    {
        public string Key => "serper-api";

        private readonly HttpClient _httpClient;

        public SerperAPIClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<string> ExecuteAsync(ToolConfigDto configs)
        {
            var query = configs.Input;
            //configs.Inputs.TryGetValue("url", out dynamic query);
            
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
