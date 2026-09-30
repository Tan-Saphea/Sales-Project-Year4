using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class ProductRepository
{
    private readonly OracleDbConnection _dbConnection;

    public ProductRepository() => _dbConnection = new OracleDbConnection();
    public ProductRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<List<Product>> GetAllAsync(bool includeDeleted = false)
    {
        var list = new List<Product>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PRODUCT_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var isDeleted = Convert.ToInt32(reader["isDeleted"]) == 1;
            if (!includeDeleted && isDeleted) continue;

            list.Add(new Product
            {
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ProductName = reader["ProductName"].ToString() ?? string.Empty,
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader["Description"].ToString(),
                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader["CategoryName"].ToString(),
                Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode")) ? null : reader["Barcode"].ToString(),
                ExpiryDate = reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null : Convert.ToDateTime(reader["ExpiryDate"]),
                Qty = Convert.ToDecimal(reader["Qty"]),
                UnitPriceIn = Convert.ToDecimal(reader["UnitPriceIn"]),
                UnitPriceOut = Convert.ToDecimal(reader["UnitPriceOut"]),
                IsDeleted = isDeleted
            });
        }
        return list;
    }

    public async Task<Product?> GetByIdAsync(int productId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PRODUCT_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ProductID", OracleDbType.Int32, productId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Product
            {
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ProductName = reader["ProductName"].ToString() ?? string.Empty,
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader["Description"].ToString(),
                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader["CategoryName"].ToString(),
                Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode")) ? null : reader["Barcode"].ToString(),
                ExpiryDate = reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? null : Convert.ToDateTime(reader["ExpiryDate"]),
                Qty = Convert.ToDecimal(reader["Qty"]),
                UnitPriceIn = Convert.ToDecimal(reader["UnitPriceIn"]),
                UnitPriceOut = Convert.ToDecimal(reader["UnitPriceOut"]),
                IsDeleted = Convert.ToInt32(reader["isDeleted"]) == 1
            };
        }
        return null;
    }

    public async Task<int> InsertAsync(Product product)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PRODUCT_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ProductName", OracleDbType.Varchar2, product.ProductName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Description", OracleDbType.Varchar2, (object?)product.Description ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CategoryID", OracleDbType.Int32, product.CategoryID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Barcode", OracleDbType.Varchar2, (object?)product.Barcode ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ExpiryDate", OracleDbType.Date, (object?)product.ExpiryDate ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Qty", OracleDbType.Decimal, product.Qty, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UnitPriceIn", OracleDbType.Decimal, product.UnitPriceIn, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UnitPriceOut", OracleDbType.Decimal, product.UnitPriceOut, ParameterDirection.Input));

        var outId = new OracleParameter("p_ProductID", OracleDbType.Decimal, ParameterDirection.Output);
        command.Parameters.Add(outId);

        await command.ExecuteNonQueryAsync();

        return outId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outId.Value);
    }

    public async Task UpdateAsync(Product product)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PRODUCT_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ProductID", OracleDbType.Int32, product.ProductID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ProductName", OracleDbType.Varchar2, product.ProductName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Description", OracleDbType.Varchar2, (object?)product.Description ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CategoryID", OracleDbType.Int32, product.CategoryID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Barcode", OracleDbType.Varchar2, (object?)product.Barcode ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ExpiryDate", OracleDbType.Date, (object?)product.ExpiryDate ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Qty", OracleDbType.Decimal, product.Qty, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UnitPriceIn", OracleDbType.Decimal, product.UnitPriceIn, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UnitPriceOut", OracleDbType.Decimal, product.UnitPriceOut, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int productId)
    {
        // Soft Delete (isDeleted = 1)
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PRODUCT_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ProductID", OracleDbType.Int32, productId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task RestoreAsync(int productId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_PRODUCT_RESTORE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_ProductID", OracleDbType.Int32, productId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
