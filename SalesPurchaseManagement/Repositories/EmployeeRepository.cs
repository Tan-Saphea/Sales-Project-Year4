using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Repositories;

/// <summary>
/// Handles database communication for employee records exclusively via Oracle Stored Procedures.
/// </summary>
public class EmployeeRepository
{
    private readonly OracleDbConnection _dbConnection;

    public EmployeeRepository()
    {
        _dbConnection = new OracleDbConnection();
    }

    public EmployeeRepository(OracleDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <summary>
    /// Retrieves all employees from tblEmployees via SP_EMPLOYEE_SELECT_ALL.
    /// Excludes sensitive password data.
    /// </summary>
    public async Task<List<Employee>> GetAllAsync()
    {
        var list = new List<Employee>();

        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_EMPLOYEE_SELECT_ALL";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(cursorParam);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapEmployeeFromReader(reader));
        }

        return list;
    }

    /// <summary>
    /// Retrieves a single employee by EmployeeID via SP_EMPLOYEE_SELECT_BY_ID.
    /// </summary>
    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_EMPLOYEE_SELECT_BY_ID";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, employeeId, ParameterDirection.Input));

        var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(cursorParam);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapEmployeeFromReader(reader);
        }

        return null;
    }

    /// <summary>
    /// Inserts a new employee via SP_EMPLOYEE_INSERT and returns the generated EmployeeID.
    /// </summary>
    public async Task<int> InsertAsync(Employee employee)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_EMPLOYEE_INSERT";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_EmployeeName", OracleDbType.Varchar2, employee.EmployeeName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UserName", OracleDbType.Varchar2, employee.UserName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Password", OracleDbType.Varchar2, employee.Password ?? string.Empty, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UserType", OracleDbType.Varchar2, (object?)employee.UserType ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Role", OracleDbType.Varchar2, (object?)employee.Role ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, (object?)employee.JobTitle ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)employee.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, (object?)employee.Phone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_HomePhone", OracleDbType.Varchar2, (object?)employee.HomePhone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)employee.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_City", OracleDbType.Varchar2, (object?)employee.City ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_StateProvince", OracleDbType.Varchar2, (object?)employee.StateProvince ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ZipPostalCode", OracleDbType.Varchar2, (object?)employee.ZipPostalCode ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CountryRegion", OracleDbType.Varchar2, (object?)employee.CountryRegion ?? DBNull.Value, ParameterDirection.Input));

        var outIdParam = new OracleParameter("p_EmployeeID", OracleDbType.Decimal)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outIdParam);

        await command.ExecuteNonQueryAsync();

        if (outIdParam.Value is OracleDecimal odec)
        {
            return odec.ToInt32();
        }

        return Convert.ToInt32(outIdParam.Value);
    }

    /// <summary>
    /// Updates an existing employee via SP_EMPLOYEE_UPDATE.
    /// If Password is null or empty, the procedure keeps the existing password intact.
    /// </summary>
    public async Task UpdateAsync(Employee employee)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_EMPLOYEE_UPDATE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, employee.EmployeeID, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_EmployeeName", OracleDbType.Varchar2, employee.EmployeeName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UserName", OracleDbType.Varchar2, employee.UserName, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Password", OracleDbType.Varchar2, string.IsNullOrWhiteSpace(employee.Password) ? (object)DBNull.Value : employee.Password, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_UserType", OracleDbType.Varchar2, (object?)employee.UserType ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Role", OracleDbType.Varchar2, (object?)employee.Role ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, (object?)employee.JobTitle ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, (object?)employee.Email ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, (object?)employee.Phone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_HomePhone", OracleDbType.Varchar2, (object?)employee.HomePhone ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, (object?)employee.Address ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_City", OracleDbType.Varchar2, (object?)employee.City ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_StateProvince", OracleDbType.Varchar2, (object?)employee.StateProvince ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_ZipPostalCode", OracleDbType.Varchar2, (object?)employee.ZipPostalCode ?? DBNull.Value, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_CountryRegion", OracleDbType.Varchar2, (object?)employee.CountryRegion ?? DBNull.Value, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Deletes an employee by EmployeeID via SP_EMPLOYEE_DELETE.
    /// </summary>
    public async Task DeleteAsync(int employeeId)
    {
        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_EMPLOYEE_DELETE";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, employeeId, ParameterDirection.Input));

        await command.ExecuteNonQueryAsync();
    }

    private static Employee MapEmployeeFromReader(IDataRecord reader)
    {
        return new Employee
        {
            EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
            EmployeeName = reader["EmployeeName"].ToString() ?? string.Empty,
            UserName = reader["UserName"].ToString() ?? string.Empty,
            UserType = reader.IsDBNull(reader.GetOrdinal("UserType")) ? null : reader["UserType"].ToString(),
            Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader["Role"].ToString(),
            JobTitle = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? null : reader["JobTitle"].ToString(),
            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
            HomePhone = reader.IsDBNull(reader.GetOrdinal("HomePhone")) ? null : reader["HomePhone"].ToString(),
            Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
            City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader["City"].ToString(),
            StateProvince = reader.IsDBNull(reader.GetOrdinal("StateProvince")) ? null : reader["StateProvince"].ToString(),
            ZipPostalCode = reader.IsDBNull(reader.GetOrdinal("ZipPostalCode")) ? null : reader["ZipPostalCode"].ToString(),
            CountryRegion = reader.IsDBNull(reader.GetOrdinal("CountryRegion")) ? null : reader["CountryRegion"].ToString()
        };
    }
}
