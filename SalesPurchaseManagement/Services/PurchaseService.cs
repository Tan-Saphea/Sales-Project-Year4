using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class PurchaseService
{
    private readonly PurchaseRepository _repository;

    public PurchaseService() => _repository = new PurchaseRepository();
    public PurchaseService(PurchaseRepository repository) => _repository = repository;

    public async Task<List<Purchase>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<List<PurchaseDetail>> GetDetailsAsync(int purchaseId) => await _repository.GetDetailsAsync(purchaseId);

    public async Task<int> ProcessPurchaseAsync(int employeeId, int supplierId, int? shipperId, List<PurchaseDetail> items)
    {
        if (employeeId <= 0)
            throw new ArgumentException("A valid employee ID is required.");

        if (supplierId <= 0)
            throw new ArgumentException("Please select a Supplier.");

        if (items == null || items.Count == 0)
            throw new ArgumentException("Purchase order is empty. Please add at least one product.");

        foreach (var item in items)
        {
            if (item.ProductID <= 0)
                throw new ArgumentException("Invalid product selected.");

            if (item.PurchaseQty <= 0)
                throw new ArgumentException($"Quantity for {item.ProductName ?? "item"} must be greater than zero.");
        }

        try
        {
            return await _repository.CreateCompletePurchaseAsync(employeeId, supplierId, shipperId, items);
        }
        catch (OracleException oex) when (oex.Number == 20005)
        {
            throw new InvalidOperationException("Oracle Error: Invalid purchase quantity specified.", oex);
        }
        catch (OracleException oex) when (oex.Number == 20002)
        {
            throw new InvalidOperationException("Oracle Error: Product was not found.", oex);
        }
    }

    public async Task CancelPurchaseAsync(int purchaseId)
    {
        await _repository.DeleteAsync(purchaseId);
    }
}
