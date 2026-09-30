using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SalesPurchaseManagement.Data;

/// <summary>
/// Centralized database configuration helper that loads connection strings from appsettings.json.
/// </summary>
public static class DbConfig
{
    private static readonly string _connectionString;

    static DbConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        _connectionString = configuration.GetConnectionString("OracleDatabase")
            ?? throw new InvalidOperationException("Connection string 'OracleDatabase' not found in appsettings.json.");
    }

    /// <summary>
    /// Gets the Oracle Database connection string configured in appsettings.json.
    /// </summary>
    public static string ConnectionString => _connectionString;
}
