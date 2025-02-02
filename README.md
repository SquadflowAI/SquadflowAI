<h2>SquadflowAI</h2>

AI Agents builder and orchestrator. Allows you just with a json file without wrting any code run an agent locally using any LLMs. The platform is agnostic to any LLM.

<h2>Features and Concepts</h2>

SquadflowAI eaither configures and runs AI Agent in run time or can scaffold separate Agent code for standalone execution.
There are 3 main buidling blocks of the platform:
<h4>Agents</h4>
The principal block of the platform, has context, goals and memory. Consists of multiple actions to achieve its final goal.
<h4>Actions</h4>
Performs the defined task. Has memory. Can recive inputs and generate outputs.
<h4>Tools</h4>
Tools are 3rd party tools and connectors that can be used by an action.  Can recive inputs and generate outputs.
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
 
<h4>stockanalysis.json</h4>
</br>

```yaml
{
    "name": "Stock Research Agent",
    "mission": "Gather stock market data for AAPL for the last month, analyze trends, and deliver a detailed weekly report in PDF format via email.",
    "capabilities": [
        {
            "task": "Search for stock data on AAPL for the last month",
            "description": "Collect data on stock price movements, volume, key financial news, and market sentiment."
        },
        {
            "task": "Analyze stock data",
            "description": "Identify trends, calculate key financial indicators, and provide insights on AAPL stock performance."
        },
        {
            "task": "Generate PDF report",
            "description": "Create a professional, well-structured report summarizing AAPL stock research and insights."
        },
        {
            "task": "Email report",
            "description": "Send the PDF report to the user's email address."
        }
    ],
    "actions": [
        {
            "name": "Search Stock Data for AAPL for the Last Month",
            "description": "Search the web for the latest stock data and financial news related to AAPL for the past month.",
            "actionToExecute": "Scrape stock market data, financial reports, and news from trusted financial websites.",
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
        },
        {
            "name": "Analyze Stock Data",
            "description": "Process and analyze the gathered stock data to identify key insights.",
            "actionToExecute": "Calculate key indicators such as moving averages, RSI, and market trends; detect significant events impacting AAPL stock.",
            "tools": [
                {
                    "name": "data-analyzer",
                    "description": "Processes raw data and calculates financial metrics.",
                    "input": "text",
                    "output": "text"
                }
            ]
        },
        {
            "name": "Design HTML and Generate PDF Report",
            "description": "Create HTML and then PDF about AAPL stock trends and insights.",
            "actionToExecute": "Design HTML and export a detailed PDF report with charts, tables, and written insights.",
            "tools": [
                {
                    "name": "pdf-generator",
                    "description": "Generates a professional PDF document from structured data.",
                    "input": "html",
                    "output": "pdf"
                }
            ]
        },
        {
            "name": "Email Report",
            "description": "Send the generated PDF report to the user's email.",
            "actionToExecute": "Attach the PDF report to an email and send it to the specified address.",
            "tools": [
                {
                    "name": "gmail-client",
                    "description": "Sends emails via Gmail API with attachment.",
                    "input": "pdf",
                    "recipientEmail": "test@test.com",
                    "recipientName": "Test Test"
                }
            ]
        }
    ],
    "limitations": [
        "Relies on publicly available stock data and news; information availability may vary.",
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



