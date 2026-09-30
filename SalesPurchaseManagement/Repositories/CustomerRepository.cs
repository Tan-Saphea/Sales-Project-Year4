using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class CustomerRepository
{
    private readonly OracleDbConnection _dbConnection;

    public CustomerRepository() => _dbConnection = new OracleDbConnection();
    public CustomerRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<List<Customer>> GetAllAsync()
    {
        var list = new List<Customer>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CUSTOMER_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Customer
            {
                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                CustomerName = reader["CustomerName"].ToString() ?? string.Empty,
                JobTitle = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? null : reader["JobTitle"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                MemberID = reader.IsDBNull(reader.GetOrdinal("MemberID")) ? null : Convert.ToInt32(reader["MemberID"]),
                MemberName = reader.IsDBNull(reader.GetOrdinal("MemberName")) ? null : reader["MemberName"].ToString(),
                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? 0 : Convert.ToDecimal(reader["Discount"])
            });
        }
        return list;
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CUSTOMER_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CustomerID", OracleDbType.Int32, customerId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Customer
            {
                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                CustomerName = reader["CustomerName"].ToString() ?? string.Empty,
                JobTitle = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? null : reader["JobTitle"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                MemberID = reader.IsDBNull(reader.GetOrdinal("MemberID")) ? null : Convert.ToInt32(reader["MemberID"]),
                MemberName = reader.IsDBNull(reader.GetOrdinal("MemberName")) ? null : reader["MemberName"].ToString(),
                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? 0 : Convert.ToDecimal(reader["Discount"])
            };
        }
        return null;
    }

    public async Task<int> InsertAsync(Customer customer)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CUSTOMER_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CustomerName", OracleDbType.Varchar2, customer.CustomerName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, (object?)customer.JobTitle ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, (object?)customer.Phone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)customer.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)customer.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_MemberID", OracleDbType.Int32, (object?)customer.MemberID ?? DBNull.Value, ParameterDirection.Input));

        var outId = new OracleParameter("p_CustomerID", OracleDbType.Decimal, ParameterDirection.Output);
        command.Parameters.Add(outId);

        await command.ExecuteNonQueryAsync();

        return outId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outId.Value);
    }

    public async Task UpdateAsync(Customer customer)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CUSTOMER_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CustomerID", OracleDbType.Int32, customer.CustomerID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CustomerName", OracleDbType.Varchar2, customer.CustomerName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, (object?)customer.JobTitle ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, (object?)customer.Phone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)customer.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)customer.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_MemberID", OracleDbType.Int32, (object?)customer.MemberID ?? DBNull.Value, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int customerId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CUSTOMER_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CustomerID", OracleDbType.Int32, customerId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
