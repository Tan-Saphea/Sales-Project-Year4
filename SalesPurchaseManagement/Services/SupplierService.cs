using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class SupplierService
{
    private readonly SupplierRepository _repository;

    public SupplierService() => _repository = new SupplierRepository();
    public SupplierService(SupplierRepository repository) => _repository = repository;

    public async Task<List<Supplier>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<int> CreateAsync(Supplier supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            throw new ArgumentException("Supplier Name is required.");

        return await _repository.InsertAsync(supplier);
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        if (supplier.SupplierID <= 0)
            throw new ArgumentException("Valid Supplier ID required for update.");

        if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            throw new ArgumentException("Supplier Name is required.");

        await _repository.UpdateAsync(supplier);
    }

    public async Task DeleteAsync(int supplierId)
    {
        try
        {
            await _repository.DeleteAsync(supplierId);
        }
        catch (OracleException oex) when (oex.Number == 2292)
        {
            throw new InvalidOperationException("Cannot delete this supplier because purchase records depend on it.", oex);
        }
    }
}
