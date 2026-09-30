using System;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace SALESPURCHASEDB.Data;

/// <summary>
/// Reusable Oracle database connection manager.
/// </summary>
public class OracleDbConnection
{
    private readonly string _connectionString;

    public OracleDbConnection()
    {
        _connectionString = DatabaseConfig.ConnectionString;
    }

    public OracleDbConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and returns a new OracleConnection instance.
    /// The caller is responsible for opening and disposing the connection.
    /// </summary>
    public OracleConnection GetConnection()
    {
        return new OracleConnection(_connectionString);
    }

    /// <summary>
    /// Tests synchronous database connectivity.
    /// Properly opens, tests, and closes/disposes the connection.
    /// </summary>
    public string TestConnection()
    {
        try
        {
            using var connection = GetConnection();
            connection.Open();

            return $"[SUCCESS] Connection successfully established to Oracle Database (Service: FREEPDB1). Server Version: {connection.ServerVersion}";
        }
        catch (OracleException oex)
        {
            return $"[ORACLE ERROR] ORA-{oex.Number}: {oex.Message}";
        }
        catch (Exception ex)
        {
            return $"[GENERAL ERROR] {ex.Message}";
        }
    }

    /// <summary>
    /// Tests asynchronous database connectivity.
    /// Properly opens, tests, and closes/disposes the connection.
    /// </summary>
    public async Task<string> TestConnectionAsync()
    {
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return $"[SUCCESS] Asynchronous connection successfully established to Oracle Database. Server Version: {connection.ServerVersion}";
        }
        catch (OracleException oex)
        {
            return $"[ORACLE ERROR] ORA-{oex.Number}: {oex.Message}";
        }
        catch (Exception ex)
        {
            return $"[GENERAL ERROR] {ex.Message}";
        }
    }
}
