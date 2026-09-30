using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class SalesRepository
{
    private readonly OracleDbConnection _dbConnection;

    public SalesRepository() => _dbConnection = new OracleDbConnection();
    public SalesRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    /// <summary>
    /// Atomically creates a Sale header and inserts all line items within a single OracleTransaction.
    /// Oracle Stored Procedures validate stock and decrease inventory automatically.
    /// </summary>
    public async Task<int> CreateCompleteSaleAsync(int employeeId, int? customerId, List<SalesDetail> details)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Create Sale Header
            int salesId;
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = "SP_SALE_INSERT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, employeeId, ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("p_CustomerID", OracleDbType.Int32, (object?)customerId ?? DBNull.Value, ParameterDirection.Input));

                var outSalesId = new OracleParameter("p_SalesID", OracleDbType.Decimal, ParameterDirection.Output);
                cmd.Parameters.Add(outSalesId);

                await cmd.ExecuteNonQueryAsync();

                salesId = outSalesId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outSalesId.Value);
            }

            // 2. Add each Line Item (SP decreases product inventory)
            foreach (var item in details)
            {
                using var detailCmd = connection.CreateCommand();
                detailCmd.Transaction = transaction;
                detailCmd.CommandText = "SP_SALES_DETAIL_INSERT";
                detailCmd.CommandType = CommandType.StoredProcedure;
                detailCmd.BindByName = true;

                detailCmd.Parameters.Add(new OracleParameter("p_SalesID", OracleDbType.Int32, salesId, ParameterDirection.Input));
                detailCmd.Parameters.Add(new OracleParameter("p_ProductID", OracleDbType.Int32, item.ProductID, ParameterDirection.Input));
                detailCmd.Parameters.Add(new OracleParameter("p_QtySales", OracleDbType.Decimal, item.QtySales, ParameterDirection.Input));

                await detailCmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return salesId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        var list = new List<Sale>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SALE_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Sale
            {
                SalesID = Convert.ToInt32(reader["SalesID"]),
                SalesDate = Convert.ToDateTime(reader["SalesDate"]),
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? null : reader["EmployeeName"].ToString(),
                CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : Convert.ToInt32(reader["CustomerID"]),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader["CustomerName"].ToString(),
                TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? 0 : Convert.ToDecimal(reader["TotalAmount"])
            });
        }
        return list;
    }

    public async Task<List<SalesDetail>> GetDetailsAsync(int salesId)
    {
        var list = new List<SalesDetail>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SALES_DETAIL_SELECT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_SalesID", OracleDbType.Int32, salesId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new SalesDetail
            {
                SalesID = Convert.ToInt32(reader["SalesID"]),
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader["ProductName"].ToString(),
                QtySales = Convert.ToDecimal(reader["QtySales"]),
                UnitPriceOut = Convert.ToDecimal(reader["UnitPriceOut"])
            });
        }
        return list;
    }

    public async Task DeleteAsync(int salesId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_SALE_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_SalesID", OracleDbType.Int32, salesId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
