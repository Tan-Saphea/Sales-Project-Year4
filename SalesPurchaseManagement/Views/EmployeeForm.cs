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

public class EmployeeForm : Window
{
    private readonly EmployeeService _service;
    private List<Employee> _allEmployees = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtName;
    private readonly TextBox _txtUser;
    private readonly TextBox _txtPass;
    private readonly ComboBox _cmbType;
    private readonly TextBox _txtRole;
    private readonly TextBox _txtJobTitle;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtPhone;
    private readonly TextBox _txtHomePhone;
    private readonly TextBox _txtAddress;
    private readonly TextBox _txtCity;
    private readonly TextBox _txtState;
    private readonly TextBox _txtZip;
    private readonly TextBox _txtCountry;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public EmployeeForm()
    {
        _service = new EmployeeService();

        Title = "Employee Management";
        Width = 1050;
        Height = 720;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var root = new DockPanel { Margin = new Thickness(16) };

        // 1. Top Section: Form Inputs & Action Buttons
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

        // Grid for fields
        var gridFields = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto")
        };

        // Row 0
        AddField(gridFields, 0, 0, "ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Full Name *:", _txtName = new TextBox { Width = 150 });
        AddField(gridFields, 0, 4, "Username *:", _txtUser = new TextBox { Width = 130 });
        AddField(gridFields, 0, 6, "Password *:", _txtPass = new TextBox { PasswordChar = '*', Width = 120 });

        // Row 1
        _cmbType = new ComboBox { ItemsSource = new[] { "Staff", "Admin" }, SelectedIndex = 0, Width = 90 };
        AddField(gridFields, 1, 0, "User Type:", _cmbType);
        AddField(gridFields, 1, 2, "Role:", _txtRole = new TextBox { Width = 150 });
        AddField(gridFields, 1, 4, "Job Title:", _txtJobTitle = new TextBox { Width = 130 });
        AddField(gridFields, 1, 6, "Email:", _txtEmail = new TextBox { Width = 120 });

        // Row 2
        AddField(gridFields, 2, 0, "Phone:", _txtPhone = new TextBox { Width = 90 });
        AddField(gridFields, 2, 2, "Home Phone:", _txtHomePhone = new TextBox { Width = 150 });
        AddField(gridFields, 2, 4, "City:", _txtCity = new TextBox { Width = 130 });
        AddField(gridFields, 2, 6, "Country:", _txtCountry = new TextBox { Text = "Cambodia", Width = 120 });

        // Row 3
        AddField(gridFields, 3, 0, "Address:", _txtAddress = new TextBox { Width = 90 });
        AddField(gridFields, 3, 2, "State/Prov:", _txtState = new TextBox { Width = 150 });
        AddField(gridFields, 3, 4, "Zip Code:", _txtZip = new TextBox { Width = 130 });

        formLayout.Children.Add(gridFields);

        // Buttons
        var btnBar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        };

        var btnNew = new Button { Content = "New", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnNew.Click += (s, e) => ClearForm();

        var btnSave = new Button { Content = "Save", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnSave.Click += async (s, e) => await SaveAsync();

        var btnUpdate = new Button { Content = "Update", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnUpdate.Click += async (s, e) => await UpdateAsync();

        var btnDelete = new Button { Content = "Delete", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnDelete.Click += async (s, e) => await DeleteAsync();

        var btnClear = new Button { Content = "Clear", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClear.Click += (s, e) => ClearForm();

        var btnRefresh = new Button { Content = "Refresh", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnRefresh.Click += async (s, e) => await LoadEmployeesAsync();

        btnBar.Children.Add(btnNew);
        btnBar.Children.Add(btnSave);
        btnBar.Children.Add(btnUpdate);
        btnBar.Children.Add(btnDelete);
        btnBar.Children.Add(btnClear);
        btnBar.Children.Add(btnRefresh);

        formLayout.Children.Add(btnBar);
        formContainer.Child = formLayout;

        // 2. Middle Section: Search Filter
        var searchPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 10)
        };
        DockPanel.SetDock(searchPanel, Dock.Top);

        searchPanel.Children.Add(new TextBlock { Text = "Search Employees:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 360, Watermark = "Filter by name, username, phone, or email..." };
        _txtSearch.TextChanged += (s, e) => FilterList();
        searchPanel.Children.Add(_txtSearch);

        // 3. Bottom Section: DataGrid
        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Avalonia.Data.Binding("EmployeeID"), Width = new DataGridLength(50) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Avalonia.Data.Binding("EmployeeName"), Width = new DataGridLength(140) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Username", Binding = new Avalonia.Data.Binding("UserName"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "User Type", Binding = new Avalonia.Data.Binding("UserType"), Width = new DataGridLength(80) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Role", Binding = new Avalonia.Data.Binding("Role"), Width = new DataGridLength(110) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Job Title", Binding = new Avalonia.Data.Binding("JobTitle"), Width = new DataGridLength(120) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Avalonia.Data.Binding("Email"), Width = new DataGridLength(150) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Phone", Binding = new Avalonia.Data.Binding("Phone"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "City", Binding = new Avalonia.Data.Binding("City"), Width = new DataGridLength(90) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Country", Binding = new Avalonia.Data.Binding("CountryRegion"), Width = new DataGridLength(90) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Employee emp)
            {
                _txtId.Text = emp.EmployeeID.ToString();
                _txtName.Text = emp.EmployeeName;
                _txtUser.Text = emp.UserName;
                _txtPass.Text = string.Empty; // Do not display password
                _cmbType.SelectedItem = emp.UserType ?? "Staff";
                _txtRole.Text = emp.Role ?? string.Empty;
                _txtJobTitle.Text = emp.JobTitle ?? string.Empty;
                _txtEmail.Text = emp.Email ?? string.Empty;
                _txtPhone.Text = emp.Phone ?? string.Empty;
                _txtHomePhone.Text = emp.HomePhone ?? string.Empty;
                _txtAddress.Text = emp.Address ?? string.Empty;
                _txtCity.Text = emp.City ?? string.Empty;
                _txtState.Text = emp.StateProvince ?? string.Empty;
                _txtZip.Text = emp.ZipPostalCode ?? string.Empty;
                _txtCountry.Text = emp.CountryRegion ?? string.Empty;
            }
        };

        root.Children.Add(formContainer);
        root.Children.Add(searchPanel);
        root.Children.Add(_grid);

        Content = root;

        Loaded += async (s, e) => await LoadEmployeesAsync();
    }

    private static void AddField(Grid g, int row, int col, string label, Control control)
    {
        var lbl = new TextBlock
        {
            Text = label,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(6, 4, 4, 4)
        };
        control.Margin = new Thickness(0, 4, 10, 4);

        Grid.SetRow(lbl, row);
        Grid.SetColumn(lbl, col);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, col + 1);

        g.Children.Add(lbl);
        g.Children.Add(control);
    }

    private async Task LoadEmployeesAsync()
    {
        try
        {
            _allEmployees = await _service.GetAllAsync();
            _grid.ItemsSource = _allEmployees;
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, $"Error: {ex.Message}", "Database Error");
        }
    }

    private void FilterList()
    {
        string q = _txtSearch.Text?.Trim().ToLower() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(q))
        {
            _grid.ItemsSource = _allEmployees;
            return;
        }

        _grid.ItemsSource = _allEmployees.Where(e =>
            (e.EmployeeName?.ToLower().Contains(q) ?? false) ||
            (e.UserName?.ToLower().Contains(q) ?? false) ||
            (e.Phone?.ToLower().Contains(q) ?? false) ||
            (e.Email?.ToLower().Contains(q) ?? false) ||
            (e.Role?.ToLower().Contains(q) ?? false)
        ).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            var emp = BuildModel();
            int newId = await _service.CreateAsync(emp);
            await MessageBoxDialog.ShowAsync(this, $"Employee saved successfully with ID: {newId}", "Success");
            await LoadEmployeesAsync();
            ClearForm();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Validation/Database Error");
        }
    }

    private async Task UpdateAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtId.Text) || !int.TryParse(_txtId.Text, out int id) || id <= 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Please select an employee row to update.", "Selection Required");
            return;
        }

        try
        {
            var emp = BuildModel();
            emp.EmployeeID = id;
            await _service.UpdateAsync(emp);
            await MessageBoxDialog.ShowAsync(this, "Employee updated successfully.", "Success");
            await LoadEmployeesAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private async Task DeleteAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtId.Text) || !int.TryParse(_txtId.Text, out int id) || id <= 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Please select an employee row to delete.", "Selection Required");
            return;
        }

        bool confirmed = await MessageBoxDialog.ShowConfirmAsync(this, $"Are you sure you want to delete employee #{id} ({_txtName.Text})?", "Confirm Delete");
        if (!confirmed) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Employee deleted successfully.", "Success");
            await LoadEmployeesAsync();
            ClearForm();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private Employee BuildModel()
    {
        return new Employee
        {
            EmployeeName = _txtName.Text?.Trim() ?? string.Empty,
            UserName = _txtUser.Text?.Trim() ?? string.Empty,
            Password = string.IsNullOrWhiteSpace(_txtPass.Text) ? null : _txtPass.Text.Trim(),
            UserType = _cmbType.SelectedItem?.ToString() ?? "Staff",
            Role = _txtRole.Text?.Trim(),
            JobTitle = _txtJobTitle.Text?.Trim(),
            Email = _txtEmail.Text?.Trim(),
            Phone = _txtPhone.Text?.Trim(),
            HomePhone = _txtHomePhone.Text?.Trim(),
            Address = _txtAddress.Text?.Trim(),
            City = _txtCity.Text?.Trim(),
            StateProvince = _txtState.Text?.Trim(),
            ZipPostalCode = _txtZip.Text?.Trim(),
            CountryRegion = _txtCountry.Text?.Trim()
        };
    }

    private void ClearForm()
    {
        _txtId.Text = string.Empty;
        _txtName.Text = string.Empty;
        _txtUser.Text = string.Empty;
        _txtPass.Text = string.Empty;
        _cmbType.SelectedIndex = 0;
        _txtRole.Text = string.Empty;
        _txtJobTitle.Text = string.Empty;
        _txtEmail.Text = string.Empty;
        _txtPhone.Text = string.Empty;
        _txtHomePhone.Text = string.Empty;
        _txtAddress.Text = string.Empty;
        _txtCity.Text = string.Empty;
        _txtState.Text = string.Empty;
        _txtZip.Text = string.Empty;
        _txtCountry.Text = "Cambodia";
        _grid.SelectedItem = null;
    }
}
