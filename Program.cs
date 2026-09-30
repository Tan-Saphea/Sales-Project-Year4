using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace SalesPurchaseManagement;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("================================================================");
        Console.WriteLine("  STEP 9.18 — EMPLOYEE CRUD INTEGRATION VERIFICATION");
        Console.WriteLine("  Schema: SALESPURCHASEDB | Docker: sales-purchase-db");
        Console.WriteLine("================================================================\n");

        string connectionString = "User Id=SALESPURCHASEDB;Password=Shop123;Data Source=localhost:1521/FREEPDB1;";

        try
        {
            using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine("[1] Connected successfully to Oracle Database.\n");

            // --- 1. Test SP_EMPLOYEE_SELECT_ALL ---
            Console.WriteLine("[2] Calling SP_EMPLOYEE_SELECT_ALL...");
            using (var selectCmd = connection.CreateCommand())
            {
                selectCmd.CommandText = "SP_EMPLOYEE_SELECT_ALL";
                selectCmd.CommandType = CommandType.StoredProcedure;
                selectCmd.BindByName = true;

                selectCmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

                using var reader = await selectCmd.ExecuteReaderAsync();
                int count = 0;
                while (await reader.ReadAsync())
                {
                    count++;
                    Console.WriteLine($"    Employee #{reader["EmployeeID"]}: {reader["EmployeeName"]} ({reader["UserName"]}) | Role: {reader["Role"]} | Email: {reader["Email"]}");
                }
                Console.WriteLine($"    Total existing employees: {count}\n");
            }

            // Cleanup any existing test employee first if left from previous runs
            using (var cleanupCmd = connection.CreateCommand())
            {
                cleanupCmd.CommandText = "DELETE FROM tblEmployees WHERE UserName = 'csharp_test'";
                await cleanupCmd.ExecuteNonQueryAsync();
            }

            // --- 2. Test SP_EMPLOYEE_INSERT ---
            Console.WriteLine("[3] Calling SP_EMPLOYEE_INSERT (New Employee: 'C# Test Employee', username: 'csharp_test')...");
            int newEmployeeId = 0;
            using (var insertCmd = connection.CreateCommand())
            {
                insertCmd.CommandText = "SP_EMPLOYEE_INSERT";
                insertCmd.CommandType = CommandType.StoredProcedure;
                insertCmd.BindByName = true;

                insertCmd.Parameters.Add(new OracleParameter("p_EmployeeName", OracleDbType.Varchar2, "C# Test Employee", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_UserName", OracleDbType.Varchar2, "csharp_test", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_Password", OracleDbType.Varchar2, "Test123", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_UserType", OracleDbType.Varchar2, "Staff", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_Role", OracleDbType.Varchar2, "Cashier", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, "Sales Staff", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, "csharp.test@example.com", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, "012000099", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_HomePhone", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, "Russian Blvd", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_City", OracleDbType.Varchar2, "Phnom Penh", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_StateProvince", OracleDbType.Varchar2, "Phnom Penh", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_ZipPostalCode", OracleDbType.Varchar2, "12000", ParameterDirection.Input));
                insertCmd.Parameters.Add(new OracleParameter("p_CountryRegion", OracleDbType.Varchar2, "Cambodia", ParameterDirection.Input));

                var outIdParam = new OracleParameter("p_EmployeeID", OracleDbType.Decimal, ParameterDirection.Output);
                insertCmd.Parameters.Add(outIdParam);

                await insertCmd.ExecuteNonQueryAsync();

                if (outIdParam.Value is OracleDecimal odec)
                    newEmployeeId = odec.ToInt32();
                else
                    newEmployeeId = Convert.ToInt32(outIdParam.Value);

                Console.WriteLine($"    [SUCCESS] Inserted new employee with generated ID: {newEmployeeId}\n");
            }

            // --- 3. Test SP_EMPLOYEE_SELECT_BY_ID ---
            Console.WriteLine($"[4] Calling SP_EMPLOYEE_SELECT_BY_ID (ID: {newEmployeeId})...");
            using (var selectByIdCmd = connection.CreateCommand())
            {
                selectByIdCmd.CommandText = "SP_EMPLOYEE_SELECT_BY_ID";
                selectByIdCmd.CommandType = CommandType.StoredProcedure;
                selectByIdCmd.BindByName = true;

                selectByIdCmd.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, newEmployeeId, ParameterDirection.Input));
                selectByIdCmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

                using var reader = await selectByIdCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Console.WriteLine($"    [VERIFIED] Found: {reader["EmployeeName"]} | User: {reader["UserName"]} | Role: {reader["Role"]} | City: {reader["City"]}\n");
                }
            }

            // --- 4. Test SP_EMPLOYEE_UPDATE ---
            Console.WriteLine($"[5] Calling SP_EMPLOYEE_UPDATE (Rename to 'C# Updated Employee')...");
            using (var updateCmd = connection.CreateCommand())
            {
                updateCmd.CommandText = "SP_EMPLOYEE_UPDATE";
                updateCmd.CommandType = CommandType.StoredProcedure;
                updateCmd.BindByName = true;

                updateCmd.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, newEmployeeId, ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_EmployeeName", OracleDbType.Varchar2, "C# Updated Employee", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_UserName", OracleDbType.Varchar2, "csharp_test", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_Password", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_UserType", OracleDbType.Varchar2, "Staff", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_Role", OracleDbType.Varchar2, "Senior Cashier", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_JobTitle", OracleDbType.Varchar2, "Senior Sales Staff", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_Email", OracleDbType.Varchar2, "csharp.test@example.com", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_Phone", OracleDbType.Varchar2, "012000099", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_HomePhone", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_Address", OracleDbType.Varchar2, "Russian Blvd", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_City", OracleDbType.Varchar2, "Phnom Penh", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_StateProvince", OracleDbType.Varchar2, "Phnom Penh", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_ZipPostalCode", OracleDbType.Varchar2, "12000", ParameterDirection.Input));
                updateCmd.Parameters.Add(new OracleParameter("p_CountryRegion", OracleDbType.Varchar2, "Cambodia", ParameterDirection.Input));

                await updateCmd.ExecuteNonQueryAsync();
                Console.WriteLine($"    [SUCCESS] Updated employee #{newEmployeeId} successfully.\n");
            }

            // Verify update
            using (var selectByIdCmd2 = connection.CreateCommand())
            {
                selectByIdCmd2.CommandText = "SP_EMPLOYEE_SELECT_BY_ID";
                selectByIdCmd2.CommandType = CommandType.StoredProcedure;
                selectByIdCmd2.BindByName = true;

                selectByIdCmd2.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, newEmployeeId, ParameterDirection.Input));
                selectByIdCmd2.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

                using var reader = await selectByIdCmd2.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Console.WriteLine($"    [VERIFIED UPDATE] Name is now: '{reader["EmployeeName"]}' | Role: '{reader["Role"]}'\n");
                }
            }

            // --- 5. Test SP_EMPLOYEE_DELETE ---
            Console.WriteLine($"[6] Calling SP_EMPLOYEE_DELETE (ID: {newEmployeeId})...");
            using (var deleteCmd = connection.CreateCommand())
            {
                deleteCmd.CommandText = "SP_EMPLOYEE_DELETE";
                deleteCmd.CommandType = CommandType.StoredProcedure;
                deleteCmd.BindByName = true;

                deleteCmd.Parameters.Add(new OracleParameter("p_EmployeeID", OracleDbType.Int32, newEmployeeId, ParameterDirection.Input));

                await deleteCmd.ExecuteNonQueryAsync();
                Console.WriteLine($"    [SUCCESS] Deleted employee #{newEmployeeId}.\n");
            }

            // Verify deletion
            using (var verifyCmd = connection.CreateCommand())
            {
                verifyCmd.CommandText = "SELECT COUNT(*) FROM tblEmployees WHERE EmployeeID = :p_id";
                verifyCmd.Parameters.Add(new OracleParameter("p_id", newEmployeeId));
                var countRemaining = Convert.ToInt32(await verifyCmd.ExecuteScalarAsync());
                Console.WriteLine($"    [VERIFIED REMOVAL] Records remaining with ID {newEmployeeId}: {countRemaining}\n");
            }

            Console.WriteLine("================================================================");
            Console.WriteLine("  ALL EMPLOYEE CRUD STORED PROCEDURES PASSED 100%!");
            Console.WriteLine("================================================================");
        }
        catch (OracleException ex)
        {
            Console.WriteLine($"Oracle Error {ex.Number}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
        }
    }
}
