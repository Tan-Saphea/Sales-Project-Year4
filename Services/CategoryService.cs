using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SALESPURCHASEDB.Models;
using SALESPURCHASEDB.Repositories;

namespace SALESPURCHASEDB.Services;

public class CategoryService
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryService(CategoryRepository? categoryRepository = null)
    {
        _categoryRepository = categoryRepository ?? new CategoryRepository();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _categoryRepository.GetCategoriesAsync();
    }

    public List<Category> GetCategories() => GetCategoriesAsync().GetAwaiter().GetResult();

    public async Task<int> InsertCategoryAsync(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category name cannot be empty.", nameof(category.CategoryName));

        return await _categoryRepository.InsertCategoryAsync(category);
    }

    public int InsertCategory(Category category) => InsertCategoryAsync(category).GetAwaiter().GetResult();

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        if (category.CategoryID <= 0)
            throw new ArgumentException("Invalid Category ID for update.", nameof(category.CategoryID));

        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category name cannot be empty.", nameof(category.CategoryName));

        return await _categoryRepository.UpdateCategoryAsync(category);
    }

    public bool UpdateCategory(Category category) => UpdateCategoryAsync(category).GetAwaiter().GetResult();

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("Invalid Category ID for delete.", nameof(categoryId));

        return await _categoryRepository.DeleteCategoryAsync(categoryId);
    }

    public bool DeleteCategory(int categoryId) => DeleteCategoryAsync(categoryId).GetAwaiter().GetResult();
}
