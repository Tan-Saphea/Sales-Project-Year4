using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SALESPURCHASEDB.Data;
using SALESPURCHASEDB.Models;

namespace SALESPURCHASEDB.Repositories;

public class EmployeeRepository
{
    private readonly OracleDbConnection _dbConnection;

    public EmployeeRepository(OracleDbConnection? dbConnection = null)
    {
        _dbConnection = dbConnection ?? new OracleDbConnection();
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        var list = new List<Employee>();

        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_GetEmployees", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursorParam);

            using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapEmployee(reader));
            }
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in GetEmployees: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in GetEmployees: {ex.Message}");
            throw;
        }

        return list;
    }

    public List<Employee> GetEmployees() => GetEmployeesAsync().GetAwaiter().GetResult();

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_GetEmployeeById", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = employeeId;

            var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(cursorParam);

            using var reader = (OracleDataReader)await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapEmployee(reader);
            }

            return null;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in GetEmployeeById: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in GetEmployeeById: {ex.Message}");
            throw;
        }
    }

    public Employee? GetEmployeeById(int employeeId) => GetEmployeeByIdAsync(employeeId).GetAwaiter().GetResult();

    public async Task<int> InsertEmployeeAsync(Employee employee)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_InsertEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_employee_name", OracleDbType.Varchar2).Value = employee.EmployeeName;
            cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = employee.UserName;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = employee.Password;
            cmd.Parameters.Add("p_user_type", OracleDbType.Varchar2).Value = (object?)employee.UserType ?? DBNull.Value;
            cmd.Parameters.Add("p_role", OracleDbType.Varchar2).Value = (object?)employee.Role ?? DBNull.Value;
            cmd.Parameters.Add("p_job_title", OracleDbType.Varchar2).Value = (object?)employee.JobTitle ?? DBNull.Value;
            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = (object?)employee.Email ?? DBNull.Value;
            cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = (object?)employee.Phone ?? DBNull.Value;
            cmd.Parameters.Add("p_home_phone", OracleDbType.Varchar2).Value = (object?)employee.HomePhone ?? DBNull.Value;
            cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = (object?)employee.Address ?? DBNull.Value;
            cmd.Parameters.Add("p_city", OracleDbType.Varchar2).Value = (object?)employee.City ?? DBNull.Value;
            cmd.Parameters.Add("p_state_province", OracleDbType.Varchar2).Value = (object?)employee.StateProvince ?? DBNull.Value;
            cmd.Parameters.Add("p_zip_postal_code", OracleDbType.Varchar2).Value = (object?)employee.ZipPostalCode ?? DBNull.Value;
            cmd.Parameters.Add("p_country_region", OracleDbType.Varchar2).Value = (object?)employee.CountryRegion ?? DBNull.Value;

            var idParam = new OracleParameter("p_employee_id", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(idParam);

            await cmd.ExecuteNonQueryAsync();

            int newId = Convert.ToInt32(idParam.Value.ToString());
            employee.EmployeeID = newId;
            return newId;
        }
        catch (OracleException oex)
        {
            Console.WriteLine($"[Database Error] OracleException in InsertEmployee: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in InsertEmployee: {ex.Message}");
            throw;
        }
    }

    public int InsertEmployee(Employee employee) => InsertEmployeeAsync(employee).GetAwaiter().GetResult();

    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_UpdateEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = employee.EmployeeID;
            cmd.Parameters.Add("p_employee_name", OracleDbType.Varchar2).Value = employee.EmployeeName;
            cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = employee.UserName;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = employee.Password;
            cmd.Parameters.Add("p_user_type", OracleDbType.Varchar2).Value = (object?)employee.UserType ?? DBNull.Value;
            cmd.Parameters.Add("p_role", OracleDbType.Varchar2).Value = (object?)employee.Role ?? DBNull.Value;
            cmd.Parameters.Add("p_job_title", OracleDbType.Varchar2).Value = (object?)employee.JobTitle ?? DBNull.Value;
            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = (object?)employee.Email ?? DBNull.Value;
            cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = (object?)employee.Phone ?? DBNull.Value;
            cmd.Parameters.Add("p_home_phone", OracleDbType.Varchar2).Value = (object?)employee.HomePhone ?? DBNull.Value;
            cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = (object?)employee.Address ?? DBNull.Value;
            cmd.Parameters.Add("p_city", OracleDbType.Varchar2).Value = (object?)employee.City ?? DBNull.Value;
            cmd.Parameters.Add("p_state_province", OracleDbType.Varchar2).Value = (object?)employee.StateProvince ?? DBNull.Value;
            cmd.Parameters.Add("p_zip_postal_code", OracleDbType.Varchar2).Value = (object?)employee.ZipPostalCode ?? DBNull.Value;
            cmd.Parameters.Add("p_country_region", OracleDbType.Varchar2).Value = (object?)employee.CountryRegion ?? DBNull.Value;

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
            Console.WriteLine($"[Database Error] OracleException in UpdateEmployee: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in UpdateEmployee: {ex.Message}");
            throw;
        }
    }

    public bool UpdateEmployee(Employee employee) => UpdateEmployeeAsync(employee).GetAwaiter().GetResult();

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        try
        {
            using var conn = _dbConnection.GetConnection();
            await conn.OpenAsync();

            using var cmd = new OracleCommand("sp_DeleteEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = employeeId;

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
            Console.WriteLine($"[Database Error] OracleException in DeleteEmployee: ORA-{oex.Number} - {oex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Application Error] General Exception in DeleteEmployee: {ex.Message}");
            throw;
        }
    }

    public bool DeleteEmployee(int employeeId) => DeleteEmployeeAsync(employeeId).GetAwaiter().GetResult();

    private static Employee MapEmployee(OracleDataReader reader)
    {
        return new Employee
        {
            EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
            EmployeeName = reader["EmployeeName"].ToString() ?? string.Empty,
            UserName = reader["UserName"].ToString() ?? string.Empty,
            Password = reader["Password"].ToString() ?? string.Empty,
            UserType = reader["UserType"] == DBNull.Value ? null : reader["UserType"].ToString(),
            Role = reader["Role"] == DBNull.Value ? null : reader["Role"].ToString(),
            JobTitle = reader["JobTitle"] == DBNull.Value ? null : reader["JobTitle"].ToString(),
            Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
            Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"].ToString(),
            HomePhone = reader["HomePhone"] == DBNull.Value ? null : reader["HomePhone"].ToString(),
            Address = reader["Address"] == DBNull.Value ? null : reader["Address"].ToString(),
            City = reader["City"] == DBNull.Value ? null : reader["City"].ToString(),
            StateProvince = reader["StateProvince"] == DBNull.Value ? null : reader["StateProvince"].ToString(),
            ZipPostalCode = reader["ZipPostalCode"] == DBNull.Value ? null : reader["ZipPostalCode"].ToString(),
            CountryRegion = reader["CountryRegion"] == DBNull.Value ? null : reader["CountryRegion"].ToString()
        };
    }
}
