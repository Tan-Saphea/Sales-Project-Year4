using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class CustomerService
{
    private readonly CustomerRepository _repository;

    public CustomerService() => _repository = new CustomerRepository();
    public CustomerService(CustomerRepository repository) => _repository = repository;

    public async Task<List<Customer>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<int> CreateAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.CustomerName))
            throw new ArgumentException("Customer Name is required.");

        return await _repository.InsertAsync(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        if (customer.CustomerID <= 0)
            throw new ArgumentException("Valid Customer ID required for update.");

        if (string.IsNullOrWhiteSpace(customer.CustomerName))
            throw new ArgumentException("Customer Name is required.");

        await _repository.UpdateAsync(customer);
    }

    public async Task DeleteAsync(int customerId)
    {
        try
        {
            await _repository.DeleteAsync(customerId);
        }
        catch (OracleException oex) when (oex.Number == 2292)
        {
            throw new InvalidOperationException("Cannot delete this customer because they have existing sales transaction records.", oex);
        }
    }
}
