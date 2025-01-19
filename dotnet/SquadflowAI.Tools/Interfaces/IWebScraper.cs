using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.Interfaces
{
    public interface IWebScraper 
    {
        Task<string> ScrapeWebsiteAsync(string url);
    }
}
