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

public class SupplierForm : Window
{
    private readonly SupplierService _service;
    private List<Supplier> _allSuppliers = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtName;
    private readonly TextBox _txtCompany;
    private readonly TextBox _txtJobTitle;
    private readonly TextBox _txtPhone;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtAddress;
    private readonly TextBox _txtCountry;
    private readonly TextBox _txtWebsite;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public SupplierForm()
    {
        _service = new SupplierService();

        Title = "Supplier Management";
        Width = 980;
        Height = 650;
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
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto")
        };

        AddField(gridFields, 0, 0, "Supplier ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Supplier Name *:", _txtName = new TextBox { Width = 180 });
        AddField(gridFields, 0, 4, "Company Name:", _txtCompany = new TextBox { Width = 160 });

        AddField(gridFields, 1, 0, "Job Title:", _txtJobTitle = new TextBox { Width = 90 });
        AddField(gridFields, 1, 2, "Phone:", _txtPhone = new TextBox { Width = 180 });
        AddField(gridFields, 1, 4, "Email:", _txtEmail = new TextBox { Width = 160 });

        AddField(gridFields, 2, 0, "Country:", _txtCountry = new TextBox { Width = 90, Text = "Cambodia" });
        AddField(gridFields, 2, 2, "Address:", _txtAddress = new TextBox { Width = 180 });
        AddField(gridFields, 2, 4, "Website:", _txtWebsite = new TextBox { Width = 160 });

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

        searchPanel.Children.Add(new TextBlock { Text = "Search Suppliers:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 350, Watermark = "Filter by supplier, company, or phone..." };
        _txtSearch.TextChanged += (s, e) => Filter();
        searchPanel.Children.Add(_txtSearch);

        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Avalonia.Data.Binding("SupplierID"), Width = new DataGridLength(55) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Supplier Name", Binding = new Avalonia.Data.Binding("SupplierName"), Width = new DataGridLength(180) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Company", Binding = new Avalonia.Data.Binding("CompanyName"), Width = new DataGridLength(180) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Phone", Binding = new Avalonia.Data.Binding("Phone"), Width = new DataGridLength(110) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Avalonia.Data.Binding("Email"), Width = new DataGridLength(150) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Country", Binding = new Avalonia.Data.Binding("Country"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Website", Binding = new Avalonia.Data.Binding("Website"), Width = new DataGridLength(140) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Supplier sup)
            {
                _txtId.Text = sup.SupplierID.ToString();
                _txtName.Text = sup.SupplierName;
                _txtCompany.Text = sup.CompanyName ?? string.Empty;
                _txtJobTitle.Text = sup.JobTitle ?? string.Empty;
                _txtPhone.Text = sup.Phone ?? string.Empty;
                _txtEmail.Text = sup.Email ?? string.Empty;
                _txtAddress.Text = sup.Address ?? string.Empty;
                _txtCountry.Text = sup.Country ?? string.Empty;
                _txtWebsite.Text = sup.Website ?? string.Empty;
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
        control.Margin = new Thickness(0, 4, 10, 4);
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
            _allSuppliers = await _service.GetAllAsync();
            _grid.ItemsSource = _allSuppliers;
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
            _grid.ItemsSource = _allSuppliers;
            return;
        }
        _grid.ItemsSource = _allSuppliers.Where(s =>
            s.SupplierName.ToLower().Contains(q) ||
            (s.CompanyName?.ToLower().Contains(q) ?? false) ||
            (s.Phone?.ToLower().Contains(q) ?? false)
        ).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            var sup = BuildModel();
            int newId = await _service.CreateAsync(sup);
            await MessageBoxDialog.ShowAsync(this, $"Supplier saved with ID: {newId}", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a supplier to update.", "Selection Required");
            return;
        }

        try
        {
            var sup = BuildModel();
            sup.SupplierID = id;
            await _service.UpdateAsync(sup);
            await MessageBoxDialog.ShowAsync(this, "Supplier updated.", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a supplier to delete.", "Selection Required");
            return;
        }

        if (!await MessageBoxDialog.ShowConfirmAsync(this, $"Delete supplier #{id}?", "Confirm")) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Supplier deleted.", "Success");
            await LoadDataAsync();
            Clear();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private Supplier BuildModel()
    {
        return new Supplier
        {
            SupplierName = _txtName.Text?.Trim() ?? string.Empty,
            CompanyName = _txtCompany.Text?.Trim(),
            JobTitle = _txtJobTitle.Text?.Trim(),
            Phone = _txtPhone.Text?.Trim(),
            Email = _txtEmail.Text?.Trim(),
            Address = _txtAddress.Text?.Trim(),
            Country = _txtCountry.Text?.Trim(),
            Website = _txtWebsite.Text?.Trim()
        };
    }

    private void Clear()
    {
        _txtId.Text = string.Empty;
        _txtName.Text = string.Empty;
        _txtCompany.Text = string.Empty;
        _txtJobTitle.Text = string.Empty;
        _txtPhone.Text = string.Empty;
        _txtEmail.Text = string.Empty;
        _txtAddress.Text = string.Empty;
        _txtCountry.Text = "Cambodia";
        _txtWebsite.Text = string.Empty;
        _grid.SelectedItem = null;
    }
}
