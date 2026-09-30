using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SALESPURCHASEDB.Data;
using SALESPURCHASEDB.Models;

namespace SALESPURCHASEDB.Repositories;

public class ProductRepository
{
    private readonly OracleDbConnection _dbConnection;

    public ProductRepository(OracleDbConnection? dbConnection = null)
    {
        _dbConnection = dbConnection ?? new OracleDbConnection();
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        var list = new List<Product>();

        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_GetProducts", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursorParam);

            using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapProduct(reader));
            }
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in GetProducts: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in GetProducts: {ex.Message}");
            throw;
        }

        return list;
    }

    public List<Product> GetProducts() => GetProductsAsync().GetAwaiter().GetResult();

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_GetProductById", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productId;

            var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursorParam);

            using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapProduct(reader);
            }

            return null;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in GetProductById: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in GetProductById: {ex.Message}");
            throw;
        }
    }

    public Product? GetProductById(int productId) => GetProductByIdAsync(productId).GetAwaiter().GetResult();

    public async Task<int> InsertProductAsync(Product product)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_InsertProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = product.ProductName;
            cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = (object?)product.Description ?? DBNull.Value;
            cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = product.CategoryID;
            cmd.Parameters.Add("p_barcode", OracleDbType.Varchar2).Value = (object?)product.Barcode ?? DBNull.Value;
            cmd.Parameters.Add("p_expiry_date", OracleDbType.Date).Value = (object?)product.ExpiryDate ?? DBNull.Value;
            cmd.Parameters.Add("p_qty", OracleDbType.Int32).Value = product.Qty;
            cmd.Parameters.Add("p_unit_price_in", OracleDbType.Decimal).Value = product.UnitPriceIn;
            cmd.Parameters.Add("p_unit_price_out", OracleDbType.Decimal).Value = product.UnitPriceOut;

            var idParam = new OracleParameter("p_product_id", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(idParam);

            await cmd.ExecuteNonQueryAsync();

            int newId = Convert.ToInt32(idParam.Value.ToString());
            product.ProductID = newId;
            return newId;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in InsertProduct: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in InsertProduct: {ex.Message}");
            throw;
        }
    }

    public int InsertProduct(Product product) => InsertProductAsync(product).GetAwaiter().GetResult();

    public async Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_UpdateProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = product.ProductID;
            cmd.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = product.ProductName;
            cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = (object?)product.Description ?? DBNull.Value;
            cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = product.CategoryID;
            cmd.Parameters.Add("p_barcode", OracleDbType.Varchar2).Value = (object?)product.Barcode ?? DBNull.Value;
            cmd.Parameters.Add("p_expiry_date", OracleDbType.Date).Value = (object?)product.ExpiryDate ?? DBNull.Value;
            cmd.Parameters.Add("p_qty", OracleDbType.Int32).Value = product.Qty;
            cmd.Parameters.Add("p_unit_price_in", OracleDbType.Decimal).Value = product.UnitPriceIn;
            cmd.Parameters.Add("p_unit_price_out", OracleDbType.Decimal).Value = product.UnitPriceOut;

            var rowsParam = new OracleParameter("p_rows_affected", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(rowsParam);

            await cmd.ExecuteNonQueryAsync();

            int rows = (rowsParam.Value != null && rowsParam.Value != DBNull.Value)
                ? Convert.ToInt32(rowsParam.Value.ToString())
                : 0;

            return rows > 0;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in UpdateProduct: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in UpdateProduct: {ex.Message}");
            throw;
        }
    }

    public bool UpdateProduct(Product product) => UpdateProductAsync(product).GetAwaiter().GetResult();

    public async Task<bool> DeleteProductAsync(int productId)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_DeleteProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productId;

            var rowsParam = new OracleParameter("p_rows_affected", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(rowsParam);

            await cmd.ExecuteNonQueryAsync();

            int rows = (rowsParam.Value != null && rowsParam.Value != DBNull.Value)
                ? Convert.ToInt32(rowsParam.Value.ToString())
                : 0;

            return rows > 0;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in DeleteProduct: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in DeleteProduct: {ex.Message}");
            throw;
        }
    }

    public bool DeleteProduct(int productId) => DeleteProductAsync(productId).GetAwaiter().GetResult();

    private static Product MapProduct(OracleDataReader reader)
    {
        return new Product
        {
            ProductID = Convert.ToInt32(reader["ProductID"]),
            ProductName = reader["ProductName"].ToString() ?? string.Empty,
            Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
            CategoryID = Convert.ToInt32(reader["CategoryID"]),
            Barcode = reader["Barcode"] == DBNull.Value ? null : reader["Barcode"].ToString(),
            ExpiryDate = reader["ExpiryDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ExpiryDate"]),
            Qty = Convert.ToInt32(reader["Qty"]),
            UnitPriceIn = Convert.ToDecimal(reader["UnitPriceIn"]),
            UnitPriceOut = Convert.ToDecimal(reader["UnitPriceOut"]),
            IsDeleted = Convert.ToInt32(reader["isDeleted"]),
            CategoryName = reader["CategoryName"] == DBNull.Value ? null : reader["CategoryName"].ToString()
        };
    }
}
