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

public class ShipperForm : Window
{
    private readonly ShipperService _service;
    private List<Shipper> _allShippers = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtCompany;
    private readonly TextBox _txtContactName;
    private readonly TextBox _txtContactNumber;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtAddress;
    private readonly TextBox _txtCountry;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public ShipperForm()
    {
        _service = new ShipperService();

        Title = "Shipper Management";
        Width = 920;
        Height = 620;
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
            RowDefinitions = new RowDefinitions("Auto,Auto")
        };

        AddField(gridFields, 0, 0, "Shipper ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Company Name *:", _txtCompany = new TextBox { Width = 180 });
        AddField(gridFields, 0, 4, "Contact Name:", _txtContactName = new TextBox { Width = 160 });

        AddField(gridFields, 1, 0, "Contact No:", _txtContactNumber = new TextBox { Width = 90 });
        AddField(gridFields, 1, 2, "Email:", _txtEmail = new TextBox { Width = 180 });
        AddField(gridFields, 1, 4, "Country:", _txtCountry = new TextBox { Text = "Cambodia", Width = 160 });

        formLayout.Children.Add(gridFields);

        var addrPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Margin = new Thickness(6, 4, 0, 0) };
        addrPanel.Children.Add(new TextBlock { Text = "Address:", VerticalAlignment = VerticalAlignment.Center });
        _txtAddress = new TextBox { Width = 640 };
        addrPanel.Children.Add(_txtAddress);
        formLayout.Children.Add(addrPanel);

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

        searchPanel.Children.Add(new TextBlock { Text = "Search Shippers:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 350, Watermark = "Filter by company or contact name..." };
        _txtSearch.TextChanged += (s, e) => Filter();
        searchPanel.Children.Add(_txtSearch);

        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Avalonia.Data.Binding("ShipperID"), Width = new DataGridLength(60) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Company Name", Binding = new Avalonia.Data.Binding("CompanyName"), Width = new DataGridLength(200) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Contact Person", Binding = new Avalonia.Data.Binding("ContactName"), Width = new DataGridLength(160) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Contact No", Binding = new Avalonia.Data.Binding("ContactNumber"), Width = new DataGridLength(120) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Avalonia.Data.Binding("Email"), Width = new DataGridLength(160) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Country", Binding = new Avalonia.Data.Binding("Country"), Width = new DataGridLength(100) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Shipper sh)
            {
                _txtId.Text = sh.ShipperID.ToString();
                _txtCompany.Text = sh.CompanyName;
                _txtContactName.Text = sh.ContactName ?? string.Empty;
                _txtContactNumber.Text = sh.ContactNumber ?? string.Empty;
                _txtEmail.Text = sh.Email ?? string.Empty;
                _txtAddress.Text = sh.Address ?? string.Empty;
                _txtCountry.Text = sh.Country ?? string.Empty;
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
            _allShippers = await _service.GetAllAsync();
            _grid.ItemsSource = _allShippers;
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
            _grid.ItemsSource = _allShippers;
            return;
        }
        _grid.ItemsSource = _allShippers.Where(s =>
            s.CompanyName.ToLower().Contains(q) ||
            (s.ContactName?.ToLower().Contains(q) ?? false) ||
            (s.ContactNumber?.ToLower().Contains(q) ?? false)
        ).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            var sh = new Shipper
            {
                CompanyName = _txtCompany.Text?.Trim() ?? string.Empty,
                ContactName = _txtContactName.Text?.Trim(),
                ContactNumber = _txtContactNumber.Text?.Trim(),
                Email = _txtEmail.Text?.Trim(),
                Address = _txtAddress.Text?.Trim(),
                Country = _txtCountry.Text?.Trim()
            };

            int newId = await _service.CreateAsync(sh);
            await MessageBoxDialog.ShowAsync(this, $"Shipper saved with ID: {newId}", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a shipper to update.", "Selection Required");
            return;
        }

        try
        {
            var sh = new Shipper
            {
                ShipperID = id,
                CompanyName = _txtCompany.Text?.Trim() ?? string.Empty,
                ContactName = _txtContactName.Text?.Trim(),
                ContactNumber = _txtContactNumber.Text?.Trim(),
                Email = _txtEmail.Text?.Trim(),
                Address = _txtAddress.Text?.Trim(),
                Country = _txtCountry.Text?.Trim()
            };

            await _service.UpdateAsync(sh);
            await MessageBoxDialog.ShowAsync(this, "Shipper updated.", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a shipper to delete.", "Selection Required");
            return;
        }

        if (!await MessageBoxDialog.ShowConfirmAsync(this, $"Delete shipper #{id}?", "Confirm")) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Shipper deleted.", "Success");
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
        _txtCompany.Text = string.Empty;
        _txtContactName.Text = string.Empty;
        _txtContactNumber.Text = string.Empty;
        _txtEmail.Text = string.Empty;
        _txtAddress.Text = string.Empty;
        _txtCountry.Text = "Cambodia";
        _grid.SelectedItem = null;
    }
}
