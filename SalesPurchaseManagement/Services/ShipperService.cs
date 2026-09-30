using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class ShipperService
{
    private readonly ShipperRepository _repository;

    public ShipperService() => _repository = new ShipperRepository();
    public ShipperService(ShipperRepository repository) => _repository = repository;

    public async Task<List<Shipper>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<int> CreateAsync(Shipper shipper)
    {
        if (string.IsNullOrWhiteSpace(shipper.CompanyName))
            throw new ArgumentException("Company Name is required.");

        return await _repository.InsertAsync(shipper);
    }

    public async Task UpdateAsync(Shipper shipper)
    {
        if (shipper.ShipperID <= 0)
            throw new ArgumentException("Valid Shipper ID required for update.");

        if (string.IsNullOrWhiteSpace(shipper.CompanyName))
            throw new ArgumentException("Company Name is required.");

        await _repository.UpdateAsync(shipper);
    }

    public async Task DeleteAsync(int shipperId)
    {
        try
        {
            await _repository.DeleteAsync(shipperId);
        }
        catch (OracleException oex) when (oex.Number == 2292)
        {
            throw new InvalidOperationException("Cannot delete this shipper because purchase records reference it.", oex);
        }
    }
}
