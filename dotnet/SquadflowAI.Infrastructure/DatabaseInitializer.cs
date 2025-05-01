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
            //CHECK

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

            const string checkTableUIFlowsQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'uiflows'
            );";

            const string checkTableProjectsQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'projects'
            );";

            const string checkTableUsersQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'users'
            );";

            //CREATE

            const string createTableAgents = @"CREATE TABLE agents (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    data JSONB);";

            const string createTableActionRun = @"CREATE TABLE actionRun (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    agentName VARCHAR(255),
                                    name VARCHAR(255),
                                    date TIMESTAMP,
                                    data TEXT,
                                    bytedata BYTEA);";

            const string createTableTools = @"CREATE TABLE tools (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    key VARCHAR(255));";

            const string createTableUIFlows = @"CREATE TABLE uiflows (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    data JSONB);";

            const string createTableProjects = @"CREATE TABLE projects (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    createdDate TIMESTAMP,
                                    updatedDate TIMESTAMP);";

            const string createTableUsers = @"CREATE TABLE users (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    email VARCHAR(255),
                                    password VARCHAR(255),
                                    createdDate TIMESTAMP,
                                    updatedDate TIMESTAMP);";

            using var connection = _dbContext.CreateConnection();

            // Check if the table exists
            bool tableExistsAgents = connection.ExecuteScalar<bool>(checkTableAgentsQuery);
            bool tableExistsActionRun = connection.ExecuteScalar<bool>(checkTableActionRunQuery);
            bool tableExistsTools = connection.ExecuteScalar<bool>(checkTableToolsRunQuery);
            bool tableExistsUIFlows = connection.ExecuteScalar<bool>(checkTableUIFlowsQuery);
            bool tableExistsProjects = connection.ExecuteScalar<bool>(checkTableProjectsQuery);
            bool tableExistsUsers = connection.ExecuteScalar<bool>(checkTableUsersQuery);

            if (!tableExistsAgents)
            {
                connection.Execute(createTableAgents);
                Console.WriteLine("Table 'Agents' has been created.");
            }

            if (!tableExistsActionRun)
            {
                connection.Execute(createTableActionRun);
                Console.WriteLine("Table 'ActionRun' has been created.");
            }

            if (!tableExistsTools)
            {
                connection.Execute(createTableTools);
                connection.Execute(@"INSERT INTO tools (name, key) VALUES 
                   ('Data Analyzer', 'data-analyzer'), 
                   ('Gmail Client', 'gmail-client'), 
                   ('Pdf Generator', 'pdf-generator'),
                   ('Serper API', 'serper-api'), 
                   ('Web Scraper','web-scraper');");
                Console.WriteLine("Table 'Tools' has been created.");
            }

            if (!tableExistsUIFlows)
            {
                connection.Execute(createTableUIFlows);
                Console.WriteLine("Table 'UIFlows' has been created.");
            }

            if (!tableExistsProjects)
            {
                connection.Execute(createTableProjects);
                Console.WriteLine("Table 'Projects' has been created.");
            }

            if (!tableExistsUsers)
            {
                connection.Execute(createTableUsers);
                Console.WriteLine("Table 'Users' has been created.");
            }

            if (tableExistsAgents && tableExistsActionRun && tableExistsTools &&
                tableExistsUIFlows && tableExistsProjects && tableExistsUsers)
            {
                Console.WriteLine("No new tables to create.");
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
