<h2>SquadflowAI</h2>

AI Agents builder and orchestrator

<h2>Features</h2>

SquadflowAI eaither configures and runs AI Agent in run time or can scaffold separate Agent code for standalone execution

<h2>Getting Started<h2>

<h3>Pre-requisites </h3>

Download the github repo
</br>
</br>
Add to appsettings.json file in SquadflowAI.Orchestrator.CLI:
</br>
</br>
1. Postgres DB: in order to work with Squadflow install latest version of the database and add the connection string.
2. Add you OpenAI API key.
3. To all the tools you would like to use add the respective API keys. Currently we support: SerperAPI, Gmail. More tools to come
</br>
</br>
Configure the Agent:
</br>
<h4>youragent.yaml</h4>
</br>

```yaml {
    "name": "Football Stats Agent",
    "mission": "Gather football statistics for Portugal for January 2025 from the internet, analyze trends, and deliver a detailed weekly report in PDF format via email.",
    "capabilities": [
        {
            "task": "Search for football statistics for Portugal for January 2025",
            "description": "Collect data on match results, player performance, team standings, and trends."
        }
    ],
    "actions": [
        {
            "name": "Search Football Stats for Portugal for January 2025",
            "description": "Search the web for the latest football statistics from trusted sources for Portugal for January 2025.",
            "actionToExecute": "Scrape match data, player statistics, and team standings from sports websites.",
            "tools": [
                    {
                        "name": "serper-api",
                        "description": "Google Search API, organic results data from Google Search.",
                        "input": "search query in text format",
                        "output": "url or urls"
                    },
                    {
                        "name": "web-scraper",
                        "description": "Scrapes a specific website page based on url provided",
                        "input": "url",
                        "output": "html of the page"
                    }
            ]
        }
    ],
    "limitations": [
        "Relies on public sports websites; data availability may vary.",
        "Accuracy of analysis depends on data quality from trusted sources.",
        "Email delivery depends on internet connectivity and Gmail API availability."
    ]
} 
```

</br>
</br>
in AgentService.cs at line 34 change for you name file.
</br>
</br>
<h3>Run</h3>
</br>
In Program.cs in the TEST area first run only the following await agentService.CreateAgentAsync();
Second step, comment await agentService.CreateAgentAsync(); and uncomment the rest and add the name of you agent from Yaml file
</br>
</br>
Run the solution
</br>
<h2>Examples</h2>

<h2>Contribution</h2>

Contributions are welcomed.
Create a new branch > Make a new feature/improvment > Create a Pull Request.
</br>
Your inputs and/or feature requests are welcomed in Discord Channel in general or feature request channels.
</br>
Discord: https://discord.gg/xNB4fVfXUX



