using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class SupplierRepository
{
    private readonly OracleDbConnection _dbConnection;

    public SupplierRepository() => _dbConnection = new OracleDbConnection();
    public SupplierRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<List<Supplier>> GetAllAsync()
    {
        var list = new List<Supplier>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SUPPLIER_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Supplier
            {
                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                SupplierName = reader["SupplierName"].ToString() ?? string.Empty,
                JobTitle = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? null : reader["JobTitle"].ToString(),
                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader["CompanyName"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString(),
                Website = reader.IsDBNull(reader.GetOrdinal("Website")) ? null : reader["Website"].ToString()
            });
        }
        return list;
    }

    public async Task<Supplier?> GetByIdAsync(int supplierId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SUPPLIER_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_SupplierID", OracleDbType.Int32, supplierId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Supplier
            {
                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                SupplierName = reader["SupplierName"].ToString() ?? string.Empty,
                JobTitle = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? null : reader["JobTitle"].ToString(),
                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader["CompanyName"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString(),
                Website = reader.IsDBNull(reader.GetOrdinal("Website")) ? null : reader["Website"].ToString()
            };
        }
        return null;
    }

    public async Task<int> InsertAsync(Supplier supplier)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SUPPLIER_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_SupplierName", OracleDbType.Varchar2, supplier.SupplierName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, (object?)supplier.JobTitle ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CompanyName", OracleDbType.Varchar2, (object?)supplier.CompanyName ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, (object?)supplier.Phone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)supplier.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)supplier.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Country", OracleDbType.Varchar2, (object?)supplier.Country ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Website", OracleDbType.Varchar2, (object?)supplier.Website ?? DBNull.Value, ParameterDirection.Input));

        var outId = new OracleParameter("p_SupplierID", OracleDbType.Decimal, ParameterDirection.Output);
        command.Parameters.Add(outId);

        await command.ExecuteNonQueryAsync();

        return outId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outId.Value);
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SUPPLIER_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_SupplierID", OracleDbType.Int32, supplier.SupplierID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_SupplierName", OracleDbType.Varchar2, supplier.SupplierName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, (object?)supplier.JobTitle ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CompanyName", OracleDbType.Varchar2, (object?)supplier.CompanyName ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, (object?)supplier.Phone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)supplier.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)supplier.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Country", OracleDbType.Varchar2, (object?)supplier.Country ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Website", OracleDbType.Varchar2, (object?)supplier.Website ?? DBNull.Value, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int supplierId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SUPPLIER_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_SupplierID", OracleDbType.Int32, supplierId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
