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
                AND table_name = 'agents'
            );";

            const string createTableQueryConfigurations = @"CREATE TABLE IF NOT EXISTS Configurations (
                Id SERIAL PRIMARY KEY,
                EmailRecipient VARCHAR(255),
                ReportSchedule VARCHAR(255),
                TrustedSources TEXT[]);";

            const string createTableQueryAgents = @"CREATE TABLE IF NOT EXISTS Agents (
                Id SERIAL PRIMARY KEY,
                Name VARCHAR(255),
                Mission VARCHAR(255),
                ConfigurationId INTEGER REFERENCES Configurations(Id),
                Limitations TEXT[]);";

            const string createTableQueryCapabilities = @"CREATE TABLE IF NOT EXISTS Capabilities (
                Id SERIAL PRIMARY KEY,
                Task VARCHAR(255),
                UseCase VARCHAR(255),
                AgentId INTEGER REFERENCES Agents(Id) ON DELETE CASCADE);";

            const string createTableQueryActions = @"CREATE TABLE IF NOT EXISTS Actions (
                Id SERIAL PRIMARY KEY,
                Name VARCHAR(255),
                Description TEXT,
                ActionToExecute TEXT,
                Inputs TEXT[],
                Outputs TEXT[],
                Triggers TEXT[],
                AgentId INTEGER REFERENCES Agents(Id) ON DELETE CASCADE);";

            const string createTableQueryTools = @"CREATE TABLE IF NOT EXISTS Tools (
                    Id SERIAL PRIMARY KEY,
                    Name VARCHAR(255),
                    Description TEXT,
                    ActionId INTEGER REFERENCES Actions(Id) ON DELETE CASCADE);";

            const string createTableQueryActionTools = @"CREATE TABLE IF NOT EXISTS ActionTools (
                    Id SERIAL PRIMARY KEY,  
                    ActionId INTEGER REFERENCES Actions(Id) ON DELETE CASCADE,   
                    ToolId INTEGER REFERENCES Tools(Id) ON DELETE CASCADE);";

            using var connection = _dbContext.CreateConnection();

            // Check if the table exists
            bool tableExists = connection.ExecuteScalar<bool>(checkTableQuery);

            if (!tableExists)
            {
                // Create the table if it doesn't exist
                connection.Execute(createTableQueryConfigurations);
                connection.Execute(createTableQueryAgents);
                connection.Execute(createTableQueryCapabilities);
                connection.Execute(createTableQueryActions);
                connection.Execute(createTableQueryTools);
                connection.Execute(createTableQueryActionTools);
                Console.WriteLine("Table 'Agents' has been created.");
            }
            else
            {
                Console.WriteLine("Table 'Agents' already exists.");
            }
        }
    

    public static string GenerateCreateTableQuery<T>(string tableName)
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName} (");

            foreach (var property in properties)
            {
                string columnName = property.Name;
                string columnType = GetPostgresType(property.PropertyType);

                // Handle primary key or auto-increment logic
                string columnDefinition = property.Name == "Id" ? $"{columnName} {columnType} SERIAL PRIMARY KEY" : $"{columnName} {columnType}";

                queryBuilder.AppendLine($"    {columnDefinition},");
            }

            // Remove the last comma
            queryBuilder.Length -= 3;
            queryBuilder.AppendLine();
            queryBuilder.AppendLine(");");

            return queryBuilder.ToString();
        }

        private static string GetPostgresType(Type type)
        {
            return type switch
            {
                var t when t == typeof(int) || t == typeof(long) => "INTEGER",
                var t when t == typeof(bool) => "BOOLEAN",
                var t when t == typeof(DateTime) => "TIMESTAMP",
                var t when t == typeof(string) => "VARCHAR(255)",
                var t when t == typeof(decimal) || t == typeof(float) || t == typeof(double) => "NUMERIC",
                _ => throw new NotSupportedException($"Type {type.Name} is not supported")
            };
        }

    }
}
