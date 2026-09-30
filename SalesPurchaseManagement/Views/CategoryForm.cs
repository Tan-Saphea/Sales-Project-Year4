using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Security;
using SalesPurchaseManagement.Services;
using SalesPurchaseManagement.Views.Dialogs;

namespace SalesPurchaseManagement.Views;

public class CategoryForm : Window
{
    private readonly CategoryService _service;
    private List<Category> _allCategories = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtName;
    private readonly TextBox _txtDescription;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public CategoryForm()
    {
        _service = new CategoryService();

        Title = "Category Management";
        Width = 850;
        Height = 600;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var root = new DockPanel { Margin = new Thickness(16) };

        var formContainer = new Border
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 0, 12)
        };
        DockPanel.SetDock(formContainer, Dock.Top);

        var formLayout = new StackPanel { Spacing = 10 };

        var gridFields = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto")
        };

        AddField(gridFields, 0, 0, "Category ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Category Name *:", _txtName = new TextBox { Width = 260 });

        AddField(gridFields, 1, 0, "Description:", _txtDescription = new TextBox { Width = 500 });
        Grid.SetColumnSpan(_txtDescription, 3);

        formLayout.Children.Add(gridFields);

        var btnBar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        };

        var btnNew = new Button { Content = "New", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnNew.Click += (s, e) => Clear();

        var btnSave = new Button { Content = "Save", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnSave.Click += async (s, e) => await SaveAsync();

        var btnUpdate = new Button { Content = "Update", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnUpdate.Click += async (s, e) => await UpdateAsync();

        var btnDelete = new Button { Content = "Delete", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnDelete.Click += async (s, e) => await DeleteAsync();

        var btnClear = new Button { Content = "Clear", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClear.Click += (s, e) => Clear();

        var btnRefresh = new Button { Content = "Refresh", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnRefresh.Click += async (s, e) => await LoadDataAsync();

        btnBar.Children.Add(btnNew);
        btnBar.Children.Add(btnSave);
        btnBar.Children.Add(btnUpdate);
        btnBar.Children.Add(btnDelete);
        btnBar.Children.Add(btnClear);
        btnBar.Children.Add(btnRefresh);

        formLayout.Children.Add(btnBar);
        formContainer.Child = formLayout;

        var searchPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 10)
        };
        DockPanel.SetDock(searchPanel, Dock.Top);

        searchPanel.Children.Add(new TextBlock { Text = "Search Categories:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 320, Watermark = "Filter category name..." };
        _txtSearch.TextChanged += (s, e) => Filter();
        searchPanel.Children.Add(_txtSearch);

        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Avalonia.Data.Binding("CategoryID"), Width = new DataGridLength(60) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Category Name", Binding = new Avalonia.Data.Binding("CategoryName"), Width = new DataGridLength(200) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Description", Binding = new Avalonia.Data.Binding("Description"), Width = new DataGridLength(300) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Created By", Binding = new Avalonia.Data.Binding("CreatedByName"), Width = new DataGridLength(140) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Created Date", Binding = new Avalonia.Data.Binding("CreatedDate"), Width = new DataGridLength(150) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Category c)
            {
                _txtId.Text = c.CategoryID.ToString();
                _txtName.Text = c.CategoryName;
                _txtDescription.Text = c.Description ?? string.Empty;
            }
        };

        root.Children.Add(formContainer);
        root.Children.Add(searchPanel);
        root.Children.Add(_grid);

        Content = root;
        Loaded += async (s, e) => await LoadDataAsync();
    }

    private static void AddField(Grid g, int row, int col, string label, Control control)
    {
        var lbl = new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 4, 4, 4) };
        control.Margin = new Thickness(0, 4, 12, 4);
        Grid.SetRow(lbl, row);
        Grid.SetColumn(lbl, col);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, col + 1);
        g.Children.Add(lbl);
        g.Children.Add(control);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _allCategories = await _service.GetAllAsync();
            _grid.ItemsSource = _allCategories;
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private void Filter()
    {
        string q = _txtSearch.Text?.Trim().ToLower() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(q))
        {
            _grid.ItemsSource = _allCategories;
            return;
        }
        _grid.ItemsSource = _allCategories.Where(c => c.CategoryName.ToLower().Contains(q) || (c.Description?.ToLower().Contains(q) ?? false)).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            var cat = new Category
            {
                CategoryName = _txtName.Text?.Trim() ?? string.Empty,
                Description = _txtDescription.Text?.Trim(),
                CreatedBy = EmployeeSession.CurrentEmployeeID > 0 ? EmployeeSession.CurrentEmployeeID : 1
            };

            int newId = await _service.CreateAsync(cat);
            await MessageBoxDialog.ShowAsync(this, $"Category saved with ID: {newId}", "Success");
            await LoadDataAsync();
            Clear();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private async Task UpdateAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtId.Text) || !int.TryParse(_txtId.Text, out int id))
        {
            await MessageBoxDialog.ShowAsync(this, "Select a category to update.", "Selection Required");
            return;
        }

        try
        {
            var cat = new Category
            {
                CategoryID = id,
                CategoryName = _txtName.Text?.Trim() ?? string.Empty,
                Description = _txtDescription.Text?.Trim(),
                UpdateBy = EmployeeSession.CurrentEmployeeID > 0 ? EmployeeSession.CurrentEmployeeID : 1
            };

            await _service.UpdateAsync(cat);
            await MessageBoxDialog.ShowAsync(this, "Category updated.", "Success");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private async Task DeleteAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtId.Text) || !int.TryParse(_txtId.Text, out int id))
        {
            await MessageBoxDialog.ShowAsync(this, "Select a category to delete.", "Selection Required");
            return;
        }

        if (!await MessageBoxDialog.ShowConfirmAsync(this, $"Delete category #{id}?", "Confirm")) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Category deleted.", "Success");
            await LoadDataAsync();
            Clear();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private void Clear()
    {
        _txtId.Text = string.Empty;
        _txtName.Text = string.Empty;
        _txtDescription.Text = string.Empty;
        _grid.SelectedItem = null;
    }
}
