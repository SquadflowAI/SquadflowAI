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
            const string checkTableAgentsQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'agents'
            );";

            const string checkTableActionRunQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'actionrun'
            );";

            const string checkTableToolsRunQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'tools'
            );";

            const string createTableQueryAgents = @"CREATE TABLE agents (
                                    id SERIAL PRIMARY KEY,
                                    name VARCHAR(255),
                                    data JSONB);";

            const string createTableQueryActionRun = @"CREATE TABLE actionRun (
                                    id SERIAL PRIMARY KEY,
                                    agentName VARCHAR(255),
                                    name VARCHAR(255),
                                    date TIMESTAMP,
                                    data TEXT,
                                    bytedata BYTEA);";

            const string createTableQueryTools = @"CREATE TABLE tools (
                                    id SERIAL PRIMARY KEY,
                                    name VARCHAR(255),
                                    key VARCHAR(255));";

            using var connection = _dbContext.CreateConnection();

            // Check if the table exists
            bool tableExistsAgents = connection.ExecuteScalar<bool>(checkTableAgentsQuery);
            bool tableExistsActionRun = connection.ExecuteScalar<bool>(checkTableActionRunQuery);
            bool tableExistsToolsRun = connection.ExecuteScalar<bool>(checkTableToolsRunQuery);

            if (!tableExistsAgents)
            {
                // Create the table if it doesn't exist
                connection.Execute(createTableQueryAgents);
 
                Console.WriteLine("Table 'Agents' has been created.");
            }else if (!tableExistsActionRun)
            {
                connection.Execute(createTableQueryActionRun);

                Console.WriteLine("Table 'ActionRun' has been created.");
            }
            else if (!tableExistsToolsRun)
            {
                connection.Execute(createTableQueryTools);

                connection.Execute(@"INSERT INTO tools (name, key) VALUES 
                                ('Data Analyzer', 'data-analyzer'), 
                                ('Gmail Client', 'gmail-client'), 
                                ('Pdf Generator', 'pdf-generator'),
                                ('Serper API', 'serper-api'), 
                                ('Web Scraper','web-scraper');");

                Console.WriteLine("Table 'ActionRun' has been created.");
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
