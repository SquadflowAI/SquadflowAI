using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace SquadflowAI.Infrastructure
{

    public class DatabaseInitializer
    {
        private readonly DbContext _dbContext;

        public DatabaseInitializer(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void EnsureDatabaseSetup()
        {
            const string checkTableQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'agentconfiguration'
            );";

            const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS AgentConfiguration (
                Id SERIAL PRIMARY KEY,
                AgentName VARCHAR(100) NOT NULL,
                IsEnabled BOOLEAN NOT NULL,
                CreatedAt TIMESTAMP DEFAULT NOW()
            );";

            using var connection = _dbContext.CreateConnection();

            // Check if the table exists
            bool tableExists = connection.ExecuteScalar<bool>(checkTableQuery);

            if (!tableExists)
            {
                // Create the table if it doesn't exist
                connection.Execute(createTableQuery);
                Console.WriteLine("Table 'AgentConfiguration' has been created.");
            }
            else
            {
                Console.WriteLine("Table 'AgentConfiguration' already exists.");
            }
        }
    }

}
