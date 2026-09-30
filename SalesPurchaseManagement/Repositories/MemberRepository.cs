using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

public class MemberRepository
{
    private readonly OracleDbConnection _dbConnection;

    public MemberRepository() => _dbConnection = new OracleDbConnection();
    public MemberRepository(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<List<Member>> GetAllAsync()
    {
        var list = new List<Member>();
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_MEMBER_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Member
            {
                MemberID = Convert.ToInt32(reader["MemberID"]),
                MemberName = reader["MemberName"].ToString() ?? string.Empty,
                Discount = Convert.ToDecimal(reader["Discount"])
            });
        }
        return list;
    }

    public async Task<Member?> GetByIdAsync(int memberId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_MEMBER_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_MemberID", OracleDbType.Int32, memberId, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Member
            {
                MemberID = Convert.ToInt32(reader["MemberID"]),
                MemberName = reader["MemberName"].ToString() ?? string.Empty,
                Discount = Convert.ToDecimal(reader["Discount"])
            };
        }
        return null;
    }

    public async Task<int> InsertAsync(Member member)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_MEMBER_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_MemberName", OracleDbType.Varchar2, member.MemberName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Discount", OracleDbType.Decimal, member.Discount, ParameterDirection.Input));

        var outId = new OracleParameter("p_MemberID", OracleDbType.Decimal, ParameterDirection.Output);
        command.Parameters.Add(outId);

        await command.ExecuteNonQueryAsync();

        return outId.Value is OracleDecimal odec ? odec.ToInt32() : Convert.ToInt32(outId.Value);
    }

    public async Task UpdateAsync(Member member)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_MEMBER_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_MemberID", OracleDbType.Int32, member.MemberID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_MemberName", OracleDbType.Varchar2, member.MemberName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Discount", OracleDbType.Decimal, member.Discount, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int memberId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_MEMBER_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_MemberID", OracleDbType.Int32, memberId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }
}
