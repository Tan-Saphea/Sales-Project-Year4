using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace SalesPurchaseManagement.Data;

/// <summary>
/// Reusable Oracle database connection factory and diagnostic connection tester.
/// </summary>
public class OracleDbConnection
{
    private readonly string _connectionString;

    public OracleDbConnection()
    {
        _connectionString = DbConfig.ConnectionString;
    }

    public OracleDbConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and returns a new OracleConnection instance.
    /// Caller is responsible for opening and properly disposing it.
    /// </summary>
    public OracleConnection GetConnection()
    {
        return new OracleConnection(_connectionString);
    }

    /// <summary>
    /// Tests database connectivity asynchronously by executing 'SELECT USER FROM DUAL'.
    /// Returns formatted diagnostic status information.
    /// </summary>
    public async Task<string> TestConnectionAsync()
    {
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT USER FROM DUAL";
            command.CommandType = CommandType.Text;

            var currentUser = await command.ExecuteScalarAsync();

            return $"Oracle Database Connection Successful.\n\nConnected User: {currentUser}";
        }
        catch (OracleException ex)
        {
            return $"Oracle Error {ex.Number}: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Database Error: {ex.Message}";
        }
    }
}
