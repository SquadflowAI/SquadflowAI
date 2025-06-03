using Newtonsoft.Json;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Tools;
using SquadflowAI.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SquadflowAI.Tools.Serper
{
    public class SerperAPIClient : ISerperAPIClient//ITool //ISerperAPIClient
    {
        public string Key => "serper-api";

        private readonly HttpClient _httpClient;

        public SerperAPIClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;


        }

        public async Task<SerperAPIResponseDto> ExecuteAsync(string query, string serperApiKey)
        {
            //var query = configs.Input;
            //configs.Inputs.TryGetValue("url", out dynamic query);
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", $"{serperApiKey}");

            var data = new
            {
                q = query,
                gl = "gb"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            //var result = new ToolResponseDto()
            //{
            //   Data = await response.Content.ReadAsStringAsync(),
            //    DataType = Contracts.Enums.ToolDataTypeEnum.String
            //};

            var stringResult = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SerperAPIResponseDto>(stringResult);

            return result;
        }

    }
}
