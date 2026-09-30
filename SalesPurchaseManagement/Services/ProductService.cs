using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class ProductService
{
    private readonly ProductRepository _repository;

    public ProductService() => _repository = new ProductRepository();
    public ProductService(ProductRepository repository) => _repository = repository;

    public async Task<List<Product>> GetAllAsync(bool includeDeleted = false) => await _repository.GetAllAsync(includeDeleted);

    public async Task<int> CreateAsync(Product product)
    {
        ValidateProduct(product);

        try
        {
            return await _repository.InsertAsync(product);
        }
        catch (OracleException oex) when (oex.Number == 1)
        {
            throw new InvalidOperationException("A product with this Barcode already exists.", oex);
        }
    }

    public async Task UpdateAsync(Product product)
    {
        if (product.ProductID <= 0)
            throw new ArgumentException("Valid Product ID required for update.");

        ValidateProduct(product);

        try
        {
            await _repository.UpdateAsync(product);
        }
        catch (OracleException oex) when (oex.Number == 1)
        {
            throw new InvalidOperationException("A product with this Barcode already exists.", oex);
        }
    }

    public async Task DeleteAsync(int productId)
    {
        // Executes soft delete (isDeleted = 1)
        await _repository.DeleteAsync(productId);
    }

    public async Task RestoreAsync(int productId)
    {
        await _repository.RestoreAsync(productId);
    }

    private static void ValidateProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductName))
            throw new ArgumentException("Product Name is required.");

        if (product.CategoryID <= 0)
            throw new ArgumentException("Please select a Category.");

        if (product.Qty < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        if (product.UnitPriceIn < 0)
            throw new ArgumentException("Cost Price (In) cannot be negative.");

        if (product.UnitPriceOut < 0)
            throw new ArgumentException("Sale Price (Out) cannot be negative.");
    }
}
