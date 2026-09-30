using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class SalesService
{
    private readonly SalesRepository _repository;

    public SalesService() => _repository = new SalesRepository();
    public SalesService(SalesRepository repository) => _repository = repository;

    public async Task<List<Sale>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<List<SalesDetail>> GetDetailsAsync(int salesId) => await _repository.GetDetailsAsync(salesId);

    public async Task<int> ProcessSaleAsync(int employeeId, int? customerId, List<SalesDetail> items)
    {
        if (employeeId <= 0)
            throw new ArgumentException("A valid cashier/employee ID is required.");

        if (items == null || items.Count == 0)
            throw new ArgumentException("Sales cart is empty. Please add at least one product.");

        foreach (var item in items)
        {
            if (item.ProductID <= 0)
                throw new ArgumentException("Invalid product selected.");

            if (item.QtySales <= 0)
                throw new ArgumentException($"Quantity for {item.ProductName ?? "item"} must be greater than zero.");
        }

        try
        {
            return await _repository.CreateCompleteSaleAsync(employeeId, customerId, items);
        }
        catch (OracleException oex) when (oex.Number == 20001)
        {
            throw new InvalidOperationException("Oracle Stock Error: Insufficient product stock available.", oex);
        }
        catch (OracleException oex) when (oex.Number == 20002)
        {
            throw new InvalidOperationException("Oracle Error: Product was not found in database.", oex);
        }
        catch (OracleException oex) when (oex.Number == 20003)
        {
            throw new InvalidOperationException("Oracle Error: Cannot sell inactive or deleted product.", oex);
        }
        catch (OracleException oex) when (oex.Number == 20004)
        {
            throw new InvalidOperationException("Oracle Error: Invalid sales quantity specified.", oex);
        }
    }

    public async Task CancelSaleAsync(int salesId)
    {
        await _repository.DeleteAsync(salesId);
    }
}
