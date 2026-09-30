using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

/// <summary>
/// Business logic and validation layer for employee operations.
/// Translates low-level database constraints into clean user-facing error messages.
/// </summary>
public class EmployeeService
{
    private readonly EmployeeRepository _repository;

    public EmployeeService()
    {
        _repository = new EmployeeRepository();
    }

    public EmployeeService(EmployeeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Gets all employee records.
    /// </summary>
    public async Task<List<Employee>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Gets an employee record by ID.
    /// </summary>
    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            throw new ArgumentException("Employee ID must be a positive integer.", nameof(employeeId));
        }

        return await _repository.GetByIdAsync(employeeId);
    }

    /// <summary>
    /// Validates employee input and creates a new employee record.
    /// Returns the generated EmployeeID.
    /// </summary>
    public async Task<int> CreateAsync(Employee employee)
    {
        ValidateEmployee(employee, isNew: true);

        try
        {
            return await _repository.InsertAsync(employee);
        }
        catch (OracleException oex) when (oex.Number == 1) // ORA-00001: unique constraint violated
        {
            throw new InvalidOperationException("An employee with this Username or Email already exists in the system.", oex);
        }
    }

    /// <summary>
    /// Validates employee input and updates an existing employee record.
    /// </summary>
    public async Task UpdateAsync(Employee employee)
    {
        if (employee.EmployeeID <= 0)
        {
            throw new ArgumentException("A valid Employee ID is required for updating.", nameof(employee));
        }

        ValidateEmployee(employee, isNew: false);

        try
        {
            await _repository.UpdateAsync(employee);
        }
        catch (OracleException oex) when (oex.Number == 1) // ORA-00001: unique constraint violated
        {
            throw new InvalidOperationException("An employee with this Username or Email already exists in the system.", oex);
        }
        catch (OracleException oex) when (oex.Number == 20002) // ORA-20002: custom not found
        {
            throw new InvalidOperationException($"Employee with ID {employee.EmployeeID} was not found.", oex);
        }
    }

    /// <summary>
    /// Deletes an employee by ID.
    /// </summary>
    public async Task DeleteAsync(int employeeId)
    {
        if (employeeId <= 0)
        {
            throw new ArgumentException("A valid Employee ID is required for deletion.", nameof(employeeId));
        }

        try
        {
            await _repository.DeleteAsync(employeeId);
        }
        catch (OracleException oex) when (oex.Number == 2292) // ORA-02292: integrity constraint violated - child record found
        {
            throw new InvalidOperationException("Cannot delete this employee because they are referenced in other records (such as Sales, Purchases, or Categories).", oex);
        }
        catch (OracleException oex) when (oex.Number == 20002) // ORA-20002: custom not found
        {
            throw new InvalidOperationException($"Employee with ID {employeeId} was not found.", oex);
        }
    }

    private static void ValidateEmployee(Employee employee, bool isNew)
    {
        if (string.IsNullOrWhiteSpace(employee.EmployeeName))
        {
            throw new ArgumentException("Employee Name is required.");
        }

        if (string.IsNullOrWhiteSpace(employee.UserName))
        {
            throw new ArgumentException("Username is required.");
        }

        if (isNew && string.IsNullOrWhiteSpace(employee.Password))
        {
            throw new ArgumentException("Password is required when registering a new employee.");
        }

        if (!string.IsNullOrWhiteSpace(employee.Email))
        {
            if (!Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Please enter a valid email address (e.g., user@example.com).");
            }
        }
    }
}
