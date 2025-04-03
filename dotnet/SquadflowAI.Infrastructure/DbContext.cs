using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;


namespace SquadflowAI.Infrastructure
{

    public class DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("PostgresConnection");
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }

}

