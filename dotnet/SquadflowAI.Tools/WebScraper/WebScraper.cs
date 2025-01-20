using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
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

                List<string> result = new List<string>();

                if (configs.Input is string singleUrl)
                {
                    // Handle single URL directly
                    var scrapedHtml = await ScrapeAsync(singleUrl);
                    if (scrapedHtml != null)
                    {
                        result.Add(scrapedHtml);
                    }
                }
                else if (configs.Input is IEnumerable<string> urlList)
                {
                    // Handle strongly-typed list of strings (e.g., List<string>, string[])
                    int counter = 0;   
                    foreach (var currentUrl in urlList)
                    {
                        if (counter >= 10)
                        {
                            break;
                        }

                        var scrapedHtml = await ScrapeAsync(currentUrl);
                        if (scrapedHtml != null)
                        {
                            result.Add(scrapedHtml);
                        }

                        counter++;
                    }
                }
                else if (configs.Input is JToken token)
                {
                    if (token.Type == JTokenType.String)
                    {
                        // Handle single URL wrapped as a JToken
                        string singleUrlJtoken = token.ToString();
                        var scrapedHtml = await ScrapeAsync(singleUrlJtoken);
                        if (scrapedHtml != null)
                        {
                            result.Add(scrapedHtml);
                        }
                    }
                    else if (token.Type == JTokenType.Array)
                    {
                        // Handle a list of URLs wrapped as a JToken
                        int counter = 0;  
                        foreach (var currentUrl in token)
                        {
                            if (counter >= 10)
                            {
                                break;
                            }

                            string url = currentUrl.ToString();
                            var scrapedHtml = await ScrapeAsync(url);

                            if(scrapedHtml != null)
                            {
                                result.Add(scrapedHtml);
                                
                            }

                            counter++;

                        }
                    }
                    else
                    {
                        // Handle unexpected token types
                        throw new InvalidOperationException("The 'Input' must be either a string, a list of strings, or a valid JToken.");
                    }
                }
                else
                {
                    // Handle invalid types
                    throw new InvalidOperationException("The 'Input' must be either a string, a list of strings, or a valid JToken.");
                }


                if (result.Count > 0)
                {
                    var finalResult = string.Join(", NEXT SCRAPED HTML: ", result.Select((x => x)));

                    return finalResult;
                }

                return null;
            }
            catch (Exception ex)
            {
                return $"Error occurred: {ex.Message}";
            }
        }

        private async Task<string> ScrapeAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
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
            catch (TaskCanceledException ex) when (ex.CancellationToken == CancellationToken.None)
            {
                // Handle timeout specifically (HttpRequestException can be caught if it's a timeout)
                Console.WriteLine($"Request timed out for URL: {url}");
                return null;  // Return null to indicate failure and move on to the next URL
            }
            catch (HttpRequestException ex)
            {
                // Handle general HTTP request errors (e.g., network issues)
                Console.WriteLine($"Error fetching URL {url}: {ex.Message}");
                return null;  // Return null to indicate failure and move on to the next URL
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                Console.WriteLine($"Unexpected error for URL {url}: {ex.Message}");
                return null;  // Return null to indicate failure and move on to the next URL
            }
        }

    }
}
