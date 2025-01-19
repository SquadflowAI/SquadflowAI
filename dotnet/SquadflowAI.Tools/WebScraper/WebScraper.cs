using HtmlAgilityPack;
using SquadflowAI.Contracts;
using SquadflowAI.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.WebScraper
{
    public class WebScraper : ITool// IWebScraper
    {
        public string Key => "web-scraper";

        private readonly HttpClient _httpClient;
        public WebScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ExecuteAsync(ToolConfigDto configs)
        {
            try
            {
                configs.Inputs.TryGetValue("url", out dynamic url);
                // Make the HTTP request
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var htmlContent = await response.Content.ReadAsStringAsync();

                // Parse HTML using HtmlAgilityPack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // Extract text content
                var plainText = htmlDoc.DocumentNode.InnerText;

                // Clean up the extracted text
                plainText = Regex.Replace(plainText, @"[ \t]+", " "); // Replace multiple spaces/tabs with a single space
                plainText = Regex.Replace(plainText, @"\s+\n\s+", "\n"); // Clean up newlines

                return plainText.Trim();
            }
            catch (Exception ex)
            {
                return $"Error occurred: {ex.Message}";
            }
        }
    }
}
