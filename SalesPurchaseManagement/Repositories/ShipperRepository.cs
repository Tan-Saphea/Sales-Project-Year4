using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class ShipperRepository
{
    private readonly OracleDbConnection _dbConnection;

    public ShipperRepository() => _dbConnection = new OracleDbConnection();
    public ShipperRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<List<Shipper>> GetAllAsync()
    {
        var list = new List<Shipper>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SHIPPER_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Shipper
            {
                ShipperID = Convert.ToInt32(reader["ShipperID"]),
                CompanyName = reader["CompanyName"].ToString() ?? string.Empty,
                ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader["ContactName"].ToString(),
                ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? null : reader["ContactNumber"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString()
            });
        }
        return list;
    }

    public async Task<Shipper?> GetByIdAsync(int shipperId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SHIPPER_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ShipperID", OracleDbType.Int32, shipperId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Shipper
            {
                ShipperID = Convert.ToInt32(reader["ShipperID"]),
                CompanyName = reader["CompanyName"].ToString() ?? string.Empty,
                ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader["ContactName"].ToString(),
                ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? null : reader["ContactNumber"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString()
            };
        }
        return null;
    }

    public async Task<int> InsertAsync(Shipper shipper)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SHIPPER_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CompanyName", OracleDbType.Varchar2, shipper.CompanyName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ContactName", OracleDbType.Varchar2, (object?)shipper.ContactName ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ContactNumber", OracleDbType.Varchar2, (object?)shipper.ContactNumber ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)shipper.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)shipper.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Country", OracleDbType.Varchar2, (object?)shipper.Country ?? DBNull.Value, ParameterDirection.Input));

        var outId = new OracleParameter("p_ShipperID", OracleDbType.Decimal, ParameterDirection.Output);
        command.Parameters.Add(outId);

        await command.ExecuteNonQueryAsync();

        return outId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outId.Value);
    }

    public async Task UpdateAsync(Shipper shipper)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SHIPPER_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ShipperID", OracleDbType.Int32, shipper.ShipperID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CompanyName", OracleDbType.Varchar2, shipper.CompanyName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ContactName", OracleDbType.Varchar2, (object?)shipper.ContactName ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ContactNumber", OracleDbType.Varchar2, (object?)shipper.ContactNumber ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)shipper.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)shipper.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Country", OracleDbType.Varchar2, (object?)shipper.Country ?? DBNull.Value, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int shipperId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SHIPPER_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ShipperID", OracleDbType.Int32, shipperId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
