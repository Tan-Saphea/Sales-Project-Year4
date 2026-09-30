using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class CategoryRepository
{
    private readonly OracleDbConnection _dbConnection;

    public CategoryRepository() => _dbConnection = new OracleDbConnection();
    public CategoryRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<List<Category>> GetAllAsync()
    {
        var list = new List<Category>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CATEGORY_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Category
            {
                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                CategoryName = reader["CategoryName"].ToString() ?? string.Empty,
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader["Description"].ToString(),
                CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? null : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : Convert.ToInt32(reader["CreatedBy"]),
                CreatedByName = reader.IsDBNull(reader.GetOrdinal("CreatedByName")) ? null : reader["CreatedByName"].ToString(),
                UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdateDate")) ? null : Convert.ToDateTime(reader["UpdateDate"]),
                UpdateBy = reader.IsDBNull(reader.GetOrdinal("UpdateBy")) ? null : Convert.ToInt32(reader["UpdateBy"])
            });
        }
        return list;
    }

    public async Task<Category?> GetByIdAsync(int categoryId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CATEGORY_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CategoryID", OracleDbType.Int32, categoryId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Category
            {
                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                CategoryName = reader["CategoryName"].ToString() ?? string.Empty,
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader["Description"].ToString(),
                CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? null : Convert.ToDateTime(reader["CreatedDate"]),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : Convert.ToInt32(reader["CreatedBy"]),
                CreatedByName = reader.IsDBNull(reader.GetOrdinal("CreatedByName")) ? null : reader["CreatedByName"].ToString(),
                UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdateDate")) ? null : Convert.ToDateTime(reader["UpdateDate"]),
                UpdateBy = reader.IsDBNull(reader.GetOrdinal("UpdateBy")) ? null : Convert.ToInt32(reader["UpdateBy"])
            };
        }
        return null;
    }

    public async Task<int> InsertAsync(Category category)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CATEGORY_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CategoryName", OracleDbType.Varchar2, category.CategoryName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Description", OracleDbType.Varchar2, (object?)category.Description ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CreatedBy", OracleDbType.Int32, (object?)category.CreatedBy ?? DBNull.Value, ParameterDirection.Input));

        var outId = new OracleParameter("p_CategoryID", OracleDbType.Decimal, ParameterDirection.Output);
        command.Parameters.Add(outId);

        await command.ExecuteNonQueryAsync();

        return outId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outId.Value);
    }

    public async Task UpdateAsync(Category category)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CATEGORY_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CategoryID", OracleDbType.Int32, category.CategoryID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CategoryName", OracleDbType.Varchar2, category.CategoryName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Description", OracleDbType.Varchar2, (object?)category.Description ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UpdateBy", OracleDbType.Int32, (object?)category.UpdateBy ?? DBNull.Value, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int categoryId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_CATEGORY_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_CategoryID", OracleDbType.Int32, categoryId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
