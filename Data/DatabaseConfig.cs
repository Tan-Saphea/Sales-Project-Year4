using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SALESPURCHASEDB.Data;

/// <summary>
/// Reads database configuration settings from appsettings.json.
/// </summary>
public static class DatabaseConfig
{
    private static readonly string _connectionString;

    static DatabaseConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        _connectionString = configuration.GetConnectionString("OracleDbConnection")
            ?? "User Id=SALESPURCHASEDB;Password=Shop123;Data Source=localhost:1521/FREEPDB1;";
    }

    public static string ConnectionString => _connectionString;
}
