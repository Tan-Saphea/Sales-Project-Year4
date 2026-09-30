using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Security;

namespace SalesPurchaseManagement.Services;

public class AuthService
{
    private readonly OracleDbConnection _dbConnection;

    public AuthService() => _dbConnection = new OracleDbConnection();
    public AuthService(OracleDbConnection dbConnection) => _dbConnection = dbConnection;

    /// <summary>
    /// Authenticates an employee against tblEmployees using SP_EMPLOYEE_LOGIN.
    /// Does not hardcode credentials.
    /// </summary>
    public async Task<Employee?> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Please enter your Username.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Please enter your Password.");

        using var connection = _dbConnection.GetConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SP_EMPLOYEE_LOGIN";
        command.CommandType = CommandType.StoredProcedure;
        command.BindByName = true;

        command.Parameters.Add(new OracleParameter("p_UserName", OracleDbType.Varchar2, username.Trim(), ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_Password", OracleDbType.Varchar2, password, ParameterDirection.Input));
        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var emp = new Employee
            {
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                EmployeeName = reader["EmployeeName"].ToString() ?? string.Empty,
                UserName = reader["UserName"].ToString() ?? string.Empty,
                UserType = reader.IsDBNull(reader.GetOrdinal("UserType")) ? "Staff" : reader["UserType"].ToString(),
                Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? "Staff" : reader["Role"].ToString(),
                JobTitle = reader.IsDBNull(reader.GetOrdinal("JobTitle")) ? null : reader["JobTitle"].ToString(),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString()
            };

            EmployeeSession.SetSession(emp);
            return emp;
        }

        return null;
    }
}
