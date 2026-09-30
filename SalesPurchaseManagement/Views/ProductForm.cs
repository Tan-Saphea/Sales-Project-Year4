using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Services;
using SalesPurchaseManagement.Views.Dialogs;

namespace SalesPurchaseManagement.Views;

public class ProductForm : Window
{
    private readonly ProductService _service;
    private readonly CategoryService _categoryService;
    private List<Product> _allProducts = new();
    private List<Category> _allCategories = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtName;
    private readonly TextBox _txtDesc;
    private readonly ComboBox _cmbCategory;
    private readonly TextBox _txtBarcode;
    private readonly DatePicker _dtpExpiry;
    private readonly TextBox _txtQty;
    private readonly TextBox _txtPriceIn;
    private readonly TextBox _txtPriceOut;
    private readonly CheckBox _chkShowDeleted;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public ProductForm()
    {
        _service = new ProductService();
        _categoryService = new CategoryService();

        Title = "Product Management";
        Width = 1080;
        Height = 720;
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
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto")
        };

        AddField(gridFields, 0, 0, "Product ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Product Name *:", _txtName = new TextBox { Width = 180 });
        _cmbCategory = new ComboBox { Width = 150 };
        AddField(gridFields, 0, 4, "Category *:", _cmbCategory);
        AddField(gridFields, 0, 6, "Barcode:", _txtBarcode = new TextBox { Width = 140 });

        AddField(gridFields, 1, 0, "Quantity *:", _txtQty = new TextBox { Text = "0", Width = 90 });
        AddField(gridFields, 1, 2, "Cost Price ($) *:", _txtPriceIn = new TextBox { Text = "0.00", Width = 180 });
        AddField(gridFields, 1, 4, "Sale Price ($) *:", _txtPriceOut = new TextBox { Text = "0.00", Width = 150 });
        _dtpExpiry = new DatePicker { Width = 140, SelectedDate = DateTimeOffset.Now.AddYears(1) };
        AddField(gridFields, 1, 6, "Expiry Date:", _dtpExpiry);

        AddField(gridFields, 2, 0, "Description:", _txtDesc = new TextBox { Width = 460 });
        Grid.SetColumnSpan(_txtDesc, 5);

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

        var btnDelete = new Button { Content = "Delete (Soft)", Width = 100, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnDelete.Click += async (s, e) => await DeleteAsync();

        var btnRestore = new Button { Content = "Restore", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnRestore.Click += async (s, e) => await RestoreAsync();

        var btnClear = new Button { Content = "Clear", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClear.Click += (s, e) => Clear();

        var btnRefresh = new Button { Content = "Refresh", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnRefresh.Click += async (s, e) => await LoadProductsAsync();

        btnBar.Children.Add(btnNew);
        btnBar.Children.Add(btnSave);
        btnBar.Children.Add(btnUpdate);
        btnBar.Children.Add(btnDelete);
        btnBar.Children.Add(btnRestore);
        btnBar.Children.Add(btnClear);
        btnBar.Children.Add(btnRefresh);

        formLayout.Children.Add(btnBar);
        formContainer.Child = formLayout;

        var searchPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Margin = new Thickness(0, 0, 0, 10)
        };
        DockPanel.SetDock(searchPanel, Dock.Top);

        searchPanel.Children.Add(new TextBlock { Text = "Search:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 320, Watermark = "Filter by product name, category, or barcode..." };
        _txtSearch.TextChanged += (s, e) => Filter();
        searchPanel.Children.Add(_txtSearch);

        _chkShowDeleted = new CheckBox { Content = "Show Soft-Deleted Products", VerticalAlignment = VerticalAlignment.Center };
        _chkShowDeleted.IsCheckedChanged += async (s, e) => await LoadProductsAsync();
        searchPanel.Children.Add(_chkShowDeleted);

        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Avalonia.Data.Binding("ProductID"), Width = new DataGridLength(55) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Product Name", Binding = new Avalonia.Data.Binding("ProductName"), Width = new DataGridLength(180) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Category", Binding = new Avalonia.Data.Binding("CategoryName"), Width = new DataGridLength(140) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Barcode", Binding = new Avalonia.Data.Binding("Barcode"), Width = new DataGridLength(120) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Stock Qty", Binding = new Avalonia.Data.Binding("Qty"), Width = new DataGridLength(90) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Cost In ($)", Binding = new Avalonia.Data.Binding("UnitPriceIn"), Width = new DataGridLength(95) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Price Out ($)", Binding = new Avalonia.Data.Binding("UnitPriceOut"), Width = new DataGridLength(95) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Expiry Date", Binding = new Avalonia.Data.Binding("ExpiryDate"), Width = new DataGridLength(110) });
        _grid.Columns.Add(new DataGridCheckBoxColumn { Header = "Deleted?", Binding = new Avalonia.Data.Binding("IsDeleted"), Width = new DataGridLength(75) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Product p)
            {
                _txtId.Text = p.ProductID.ToString();
                _txtName.Text = p.ProductName;
                _txtDesc.Text = p.Description ?? string.Empty;
                _txtBarcode.Text = p.Barcode ?? string.Empty;
                _txtQty.Text = p.Qty.ToString();
                _txtPriceIn.Text = p.UnitPriceIn.ToString();
                _txtPriceOut.Text = p.UnitPriceOut.ToString();

                var found = _allCategories.FirstOrDefault(c => c.CategoryID == p.CategoryID);
                _cmbCategory.SelectedItem = found;

                if (p.ExpiryDate.HasValue)
                {
                    _dtpExpiry.SelectedDate = new DateTimeOffset(p.ExpiryDate.Value);
                }
            }
        };

        root.Children.Add(formContainer);
        root.Children.Add(searchPanel);
        root.Children.Add(_grid);

        Content = root;
        Loaded += async (s, e) =>
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        };
    }

    private static void AddField(Grid g, int row, int col, string label, Control control)
    {
        var lbl = new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 4, 4, 4) };
        control.Margin = new Thickness(0, 4, 10, 4);
        Grid.SetRow(lbl, row);
        Grid.SetColumn(lbl, col);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, col + 1);
        g.Children.Add(lbl);
        g.Children.Add(control);
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            _allCategories = await _categoryService.GetAllAsync();
            _cmbCategory.ItemsSource = _allCategories;
            _cmbCategory.DisplayMemberBinding = new Avalonia.Data.Binding("CategoryName");
            if (_allCategories.Count > 0) _cmbCategory.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            bool showDeleted = _chkShowDeleted.IsChecked == true;
            _allProducts = await _service.GetAllAsync(includeDeleted: showDeleted);
            _grid.ItemsSource = _allProducts;
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
            _grid.ItemsSource = _allProducts;
            return;
        }
        _grid.ItemsSource = _allProducts.Where(p =>
            p.ProductName.ToLower().Contains(q) ||
            (p.Barcode?.ToLower().Contains(q) ?? false) ||
            (p.CategoryName?.ToLower().Contains(q) ?? false)
        ).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            var p = BuildModel();
            int newId = await _service.CreateAsync(p);
            await MessageBoxDialog.ShowAsync(this, $"Product saved with ID: {newId}", "Success");
            await LoadProductsAsync();
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
            await MessageBoxDialog.ShowAsync(this, "Select a product to update.", "Selection Required");
            return;
        }

        try
        {
            var p = BuildModel();
            p.ProductID = id;
            await _service.UpdateAsync(p);
            await MessageBoxDialog.ShowAsync(this, "Product updated.", "Success");
            await LoadProductsAsync();
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
            await MessageBoxDialog.ShowAsync(this, "Select a product to delete.", "Selection Required");
            return;
        }

        if (!await MessageBoxDialog.ShowConfirmAsync(this, $"Soft-delete product #{id}? It will be marked inactive.", "Confirm Soft Delete")) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Product soft-deleted (isDeleted = 1).", "Success");
            await LoadProductsAsync();
            Clear();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private async Task RestoreAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtId.Text) || !int.TryParse(_txtId.Text, out int id))
        {
            await MessageBoxDialog.ShowAsync(this, "Select a deleted product to restore.", "Selection Required");
            return;
        }

        try
        {
            await _service.RestoreAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Product restored to active catalog.", "Success");
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private Product BuildModel()
    {
        var selCat = _cmbCategory.SelectedItem as Category;
        decimal.TryParse(_txtQty.Text, out decimal qty);
        decimal.TryParse(_txtPriceIn.Text, out decimal pIn);
        decimal.TryParse(_txtPriceOut.Text, out decimal pOut);

        DateTime? expDate = _dtpExpiry.SelectedDate?.DateTime;

        return new Product
        {
            ProductName = _txtName.Text?.Trim() ?? string.Empty,
            Description = _txtDesc.Text?.Trim(),
            CategoryID = selCat?.CategoryID ?? 0,
            Barcode = string.IsNullOrWhiteSpace(_txtBarcode.Text) ? null : _txtBarcode.Text.Trim(),
            ExpiryDate = expDate,
            Qty = qty,
            UnitPriceIn = pIn,
            UnitPriceOut = pOut
        };
    }

    private void Clear()
    {
        _txtId.Text = string.Empty;
        _txtName.Text = string.Empty;
        _txtDesc.Text = string.Empty;
        _txtBarcode.Text = string.Empty;
        _txtQty.Text = "0";
        _txtPriceIn.Text = "0.00";
        _txtPriceOut.Text = "0.00";
        _grid.SelectedItem = null;
    }
}
