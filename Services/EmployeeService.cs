using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SALESPURCHASEDB.Models;
using SALESPURCHASEDB.Repositories;

namespace SALESPURCHASEDB.Services;

public class EmployeeService
{
    private readonly EmployeeRepository _employeeRepository;

    public EmployeeService(EmployeeRepository? employeeRepository = null)
    {
        _employeeRepository = employeeRepository ?? new EmployeeRepository();
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _employeeRepository.GetEmployeesAsync();
    }

    public List<Employee> GetEmployees() => GetEmployeesAsync().GetAwaiter().GetResult();

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentException("Invalid Employee ID.", nameof(employeeId));

        return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
    }

    public Employee? GetEmployeeById(int employeeId) => GetEmployeeByIdAsync(employeeId).GetAwaiter().GetResult();

    public async Task<int> InsertEmployeeAsync(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");

        if (string.IsNullOrWhiteSpace(employee.EmployeeName))
            throw new ArgumentException("Employee name cannot be empty.", nameof(employee.EmployeeName));

        if (string.IsNullOrWhiteSpace(employee.UserName))
            throw new ArgumentException("Username cannot be empty.", nameof(employee.UserName));

        if (string.IsNullOrWhiteSpace(employee.Password))
            throw new ArgumentException("Password cannot be empty.", nameof(employee.Password));

        return await _employeeRepository.InsertEmployeeAsync(employee);
    }

    public int InsertEmployee(Employee employee) => InsertEmployeeAsync(employee).GetAwaiter().GetResult();

    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");

        if (employee.EmployeeID <= 0)
            throw new ArgumentException("Invalid Employee ID for update.", nameof(employee.EmployeeID));

        if (string.IsNullOrWhiteSpace(employee.EmployeeName))
            throw new ArgumentException("Employee name cannot be empty.", nameof(employee.EmployeeName));

        if (string.IsNullOrWhiteSpace(employee.UserName))
            throw new ArgumentException("Username cannot be empty.", nameof(employee.UserName));

        return await _employeeRepository.UpdateEmployeeAsync(employee);
    }

    public bool UpdateEmployee(Employee employee) => UpdateEmployeeAsync(employee).GetAwaiter().GetResult();

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentException("Invalid Employee ID for delete.", nameof(employeeId));

        return await _employeeRepository.DeleteEmployeeAsync(employeeId);
    }

    public bool DeleteEmployee(int employeeId) => DeleteEmployeeAsync(employeeId).GetAwaiter().GetResult();
}
