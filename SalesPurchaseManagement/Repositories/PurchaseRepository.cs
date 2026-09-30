using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class PurchaseRepository
{
    private readonly OracleDbConnection _dbConnection;

    public PurchaseRepository() => _dbConnection = new OracleDbConnection();
    public PurchaseRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    /// <summary>
    /// Atomically creates a Purchase header and inserts all line items within a single OracleTransaction.
    /// Oracle Stored Procedures automatically increment product inventory.
    /// </summary>
    public async Task<int> CreateCompletePurchaseAsync(int employeeId, int supplierId, int? shipperId, List<PurchaseDetail> details)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Create Purchase Header
            int purchaseId;
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = "SP_PURCHASE_INSERT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                cmd.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, employeeId, ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("p_SupplierID", OracleDbType.Int32, supplierId, ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("p_ShipperID", OracleDbType.Int32, (object?)shipperId ?? DBNull.Value, ParameterDirection.Input));

                var outPurId = new OracleParameter("p_PurchaseID", OracleDbType.Decimal, ParameterDirection.Output);
                cmd.Parameters.Add(outPurId);

                await cmd.ExecuteNonQueryAsync();

                purchaseId = outPurId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outPurId.Value);
            }

            // 2. Add Line Items (SP increases product inventory)
            foreach (var item in details)
            {
                using var detailCmd = connection.CreateCommand();
                detailCmd.Transaction = transaction;
                detailCmd.CommandText = "SP_PURCHASE_DETAIL_INSERT";
                detailCmd.CommandType = CommandType.StoredProcedure;
                detailCmd.BindByName = true;

                detailCmd.Parameters.Add(new OracleParameter("p_PurchaseID", OracleDbType.Int32, purchaseId, ParameterDirection.Input));
                detailCmd.Parameters.Add(new OracleParameter("p_ProductID", OracleDbType.Int32, item.ProductID, ParameterDirection.Input));
                detailCmd.Parameters.Add(new OracleParameter("p_PurchaseQty", OracleDbType.Decimal, item.PurchaseQty, ParameterDirection.Input));

                await detailCmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return purchaseId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<Purchase>> GetAllAsync()
    {
        var list = new List<Purchase>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PURCHASE_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Purchase
            {
                PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"]),
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? null : reader["EmployeeName"].ToString(),
                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader["SupplierName"].ToString(),
                ShipperID = reader.IsDBNull(reader.GetOrdinal("ShipperID")) ? null : Convert.ToInt32(reader["ShipperID"]),
                ShipperName = reader.IsDBNull(reader.GetOrdinal("ShipperName")) ? null : reader["ShipperName"].ToString(),
                TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? 0 : Convert.ToDecimal(reader["TotalAmount"])
            });
        }
        return list;
    }

    public async Task<List<PurchaseDetail>> GetDetailsAsync(int purchaseId)
    {
        var list = new List<PurchaseDetail>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PURCHASE_DETAIL_SELECT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_PurchaseID", OracleDbType.Int32, purchaseId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new PurchaseDetail
            {
                PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader["ProductName"].ToString(),
                PurchaseQty = Convert.ToDecimal(reader["PurchaseQty"]),
                UnitPriceIn = Convert.ToDecimal(reader["UnitPriceIn"])
            });
        }
        return list;
    }

    public async Task DeleteAsync(int purchaseId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PURCHASE_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_PurchaseID", OracleDbType.Int32, purchaseId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
