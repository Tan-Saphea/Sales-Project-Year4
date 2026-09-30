using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class CategoryService
{
    private readonly CategoryRepository _repository;

    public CategoryService() => _repository = new CategoryRepository();
    public CategoryService(CategoryRepository repository) => _repository = repository;

    public async Task<List<Category>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<int> CreateAsync(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category Name is required.");

        try
        {
            return await _repository.InsertAsync(category);
        }
        catch (OracleException oex) when (oex.Number == 1)
        {
            throw new InvalidOperationException("A category with this name already exists.", oex);
        }
    }

    public async Task UpdateAsync(Category category)
    {
        if (category.CategoryID <= 0)
            throw new ArgumentException("Valid Category ID required for update.");

        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category Name is required.");

        try
        {
            await _repository.UpdateAsync(category);
        }
        catch (OracleException oex) when (oex.Number == 1)
        {
            throw new InvalidOperationException("A category with this name already exists.", oex);
        }
    }

    public async Task DeleteAsync(int categoryId)
    {
        try
        {
            await _repository.DeleteAsync(categoryId);
        }
        catch (OracleException oex) when (oex.Number == 2292)
        {
            throw new InvalidOperationException("Cannot delete this category because products are assigned to it.", oex);
        }
    }
}
