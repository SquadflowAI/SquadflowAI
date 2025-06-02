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

            const string checkTableAppsSystemRunQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'appssystem'
            );";


            const string checkTableAIToolsSystemRunQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'aitoolssystem'
            );";

            const string checkTableToolsSystemRunQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'toolssystem'
            );";

            const string checkTableCoreToolsSystemRunQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'coretoolssystem'
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

            const string checkTableIntegrationsQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'integrations'
            );";

            //CREATE

            const string createTableAgents = @"CREATE TABLE agents (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    type VARCHAR(55),
                                    data JSONB);";

            const string createTableActionRun = @"CREATE TABLE actionRun (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    agentId UUID,
                                    flowId UUID,
                                    createddate TIMESTAMP,
                                    data TEXT,
                                    bytedata BYTEA);";

            const string createTableAIToolsSystem = @"CREATE TABLE aitoolssystem (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    type VARCHAR(255),
                                    key VARCHAR(255));";

            const string createTableToolsSystem = @"CREATE TABLE toolssystem (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    key VARCHAR(255));";

            const string createTableCoreToolsSystem = @"CREATE TABLE coretoolssystem (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    type VARCHAR(55),
                                    key VARCHAR(255));";

            const string createTableAppsSystem = @"CREATE TABLE appssystem (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    key VARCHAR(255));";

            const string createTableUsers = @"CREATE TABLE users (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    name VARCHAR(255),
                                    email VARCHAR(255),
                                    password VARCHAR(255),
                                    createdDate TIMESTAMP,
                                    updatedDate TIMESTAMP);";

            const string createTableProjects = @"CREATE TABLE projects (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    userId UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                                    name VARCHAR(255),
                                    createdDate TIMESTAMP,
                                    updatedDate TIMESTAMP);";

            const string createTableUIFlows = @"CREATE TABLE uiflows (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    userId UUID NOT NULL,
                                    projectId UUID NOT NULL REFERENCES projects(id) ON DELETE CASCADE,
                                    name VARCHAR(255),
                                    data JSONB);";


            const string createTableIntegrations = @"CREATE TABLE integrations (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                    userId UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                                    openAIKey VARCHAR(255),
                                    createdDate TIMESTAMP,
                                    updatedDate TIMESTAMP);";




            using var connection = _dbContext.CreateConnection();

            // Check if the table exists
            bool tableExistsAgents = connection.ExecuteScalar<bool>(checkTableAgentsQuery);
            bool tableExistsActionRun = connection.ExecuteScalar<bool>(checkTableActionRunQuery); 
            bool tableExistsAIToolsSystem = connection.ExecuteScalar<bool>(checkTableAIToolsSystemRunQuery);
            bool tableExistsToolsSystem = connection.ExecuteScalar<bool>(checkTableToolsSystemRunQuery);
            bool tableExistsCoreToolsSystem = connection.ExecuteScalar<bool>(checkTableCoreToolsSystemRunQuery);
            bool tableExistsAppsSystem = connection.ExecuteScalar<bool>(checkTableAppsSystemRunQuery);
            bool tableExistsUIFlows = connection.ExecuteScalar<bool>(checkTableUIFlowsQuery);
            bool tableExistsProjects = connection.ExecuteScalar<bool>(checkTableProjectsQuery);
            bool tableExistsUsers = connection.ExecuteScalar<bool>(checkTableUsersQuery);
            bool tableExistsIntegrations = connection.ExecuteScalar<bool>(checkTableIntegrationsQuery);

            if (!tableExistsAgents)
            {
                connection.Execute(createTableAgents);
                Console.WriteLine("Table 'Agents' has been created.");
                connection.Execute(@"INSERT INTO agents (name, data, type) VALUES 
                   ('LLM Promt', '', 'system'),
                   ('Web Research', '', 'system'),
                   ('Data search', '', 'system'),
                   ('Summarize Text', '', 'system'),
                   ('Categorizer Text', '', 'system');");
            }

            if (!tableExistsActionRun)
            {
                connection.Execute(createTableActionRun);
                Console.WriteLine("Table 'ActionRun' has been created.");
            }

            if (!tableExistsAppsSystem)
            {
                connection.Execute(createTableAppsSystem);
                connection.Execute(@"INSERT INTO appssystem (name, key) VALUES 
                   ('Gmail', 'gmail-app');");
                Console.WriteLine("Table 'appssystem' has been created.");
            }

            if (!tableExistsToolsSystem)
            {
                connection.Execute(createTableToolsSystem);
                connection.Execute(@"INSERT INTO toolssystem (name, key) VALUES 
                   ('Serper API', 'serper-api');");
                Console.WriteLine("Table 'toolssystem' has been created.");
            }

            if (!tableExistsAIToolsSystem)
            {
                connection.Execute(createTableAIToolsSystem);
                connection.Execute(@"INSERT INTO aitoolssystem (name, key, type) VALUES 
                   ('LLM Promt', 'llm-promt', 'core'),
                   ('Web Research', 'web-research', 'agent'),
                   ('Data search', 'data-search', 'agent'),
                   ('Summarize Text', 'text-summarizer', 'agent'),
                   ('Categorizer Text', 'text-summarizer', 'agent');");
                Console.WriteLine("Table 'aitoolssystem' has been created.");
            }

            if (!tableExistsCoreToolsSystem)
            {
                connection.Execute(createTableCoreToolsSystem);
                connection.Execute(@"INSERT INTO coretoolssystem (name, key, type) VALUES 
                   ('HTTP Request', 'http-request', 'core'),
                   ('Run code', 'run-code', 'core'),
                   ('HTML to PDF', 'html-to-pdf', 'core'),
                   ('Human Action', 'human-action', 'core'),
                   ('Text Input', 'text-input', 'input'),
                   ('Text Output', 'text-output', 'output'),
                   ('Pdf Output', 'pdf-output', 'output'),
                   ('Word Output', 'word-output', 'output'),
                   ('Json Output', 'json-output', 'output');");
                Console.WriteLine("Table 'coretoolssystem' has been created.");
            }

            //('Data Analyzer', 'data-analyzer'), 
            //('Pdf Generator', 'pdf-generator'),

            if (!tableExistsUsers)
            {
                connection.Execute(createTableUsers);
                Console.WriteLine("Table 'Users' has been created.");
            }

            if (!tableExistsProjects)
            {
                connection.Execute(createTableProjects);
                Console.WriteLine("Table 'Projects' has been created.");
            }

            if (!tableExistsUIFlows)
            {
                connection.Execute(createTableUIFlows);
                Console.WriteLine("Table 'UIFlows' has been created.");
            }


            if (!tableExistsIntegrations)
            {
                connection.Execute(createTableIntegrations);
                Console.WriteLine("Table 'UIFlows' has been created.");
            }

            if (tableExistsAgents && tableExistsActionRun && tableExistsCoreToolsSystem &&
                tableExistsAIToolsSystem && tableExistsToolsSystem && tableExistsAppsSystem &&
                tableExistsUIFlows && tableExistsProjects && tableExistsUsers && tableExistsIntegrations)
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
