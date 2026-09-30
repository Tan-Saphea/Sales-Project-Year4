using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SALESPURCHASEDB.Data;
using SALESPURCHASEDB.Models;

namespace SALESPURCHASEDB.Repositories;

public class CategoryRepository
{
    private readonly OracleDbConnection _dbConnection;

    public CategoryRepository(OracleDbConnection? dbConnection = null)
    {
        _dbConnection = dbConnection ?? new OracleDbConnection();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var list = new List<Category>();

        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_GetCategories", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursorParam);

            using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Category
                {
                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                    CategoryName = reader["CategoryName"].ToString() ?? string.Empty,
                    Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                    CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                    CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]),
                    UpdateDate = reader["UpdateDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdateDate"]),
                    UpdateBy = reader["UpdateBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdateBy"]),
                    CreatedByName = reader["CreatedByName"] == DBNull.Value ? null : reader["CreatedByName"].ToString()
                });
            }
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in GetCategories: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in GetCategories: {ex.Message}");
            throw;
        }

        return list;
    }

    public List<Category> GetCategories() => GetCategoriesAsync().GetAwaiter().GetResult();

    public async Task<int> InsertCategoryAsync(Category category)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_InsertCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_category_name", OracleDbType.Varchar2).Value = category.CategoryName;
            cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = (object?)category.Description ?? DBNull.Value;
            cmd.Parameters.Add("p_created_by", OracleDbType.Int32).Value = (object?)category.CreatedBy ?? DBNull.Value;

            var idParam = new OracleParameter("p_category_id", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(idParam);

            await cmd.ExecuteNonQueryAsync();

            int newId = Convert.ToInt32(idParam.Value.ToString());
            category.CategoryID = newId;
            return newId;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in InsertCategory: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in InsertCategory: {ex.Message}");
            throw;
        }
    }

    public int InsertCategory(Category category) => InsertCategoryAsync(category).GetAwaiter().GetResult();

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_UpdateCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = category.CategoryID;
            cmd.Parameters.Add("p_category_name", OracleDbType.Varchar2).Value = category.CategoryName;
            cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = (object?)category.Description ?? DBNull.Value;
            cmd.Parameters.Add("p_update_by", OracleDbType.Int32).Value = (object?)category.UpdateBy ?? DBNull.Value;

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
            Console.WriteLine($"[Database Error] OracleException in UpdateCategory: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in UpdateCategory: {ex.Message}");
            throw;
        }
    }

    public bool UpdateCategory(Category category) => UpdateCategoryAsync(category).GetAwaiter().GetResult();

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_DeleteCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = categoryId;

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
            Console.WriteLine($"[Database Error] OracleException in DeleteCategory: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in DeleteCategory: {ex.Message}");
            throw;
        }
    }

    public bool DeleteCategory(int categoryId) => DeleteCategoryAsync(categoryId).GetAwaiter().GetResult();
}
