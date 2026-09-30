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

public class CustomerForm : Window
{
    private readonly CustomerService _service;
    private readonly MemberService _memberService;
    private List<Customer> _allCustomers = new();
    private List<Member> _allMembers = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtName;
    private readonly TextBox _txtJobTitle;
    private readonly TextBox _txtPhone;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtAddress;
    private readonly ComboBox _cmbMember;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public CustomerForm()
    {
        _service = new CustomerService();
        _memberService = new MemberService();

        Title = "Customer Management";
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
            RowDefinitions = new RowDefinitions("Auto,Auto")
        };

        AddField(gridFields, 0, 0, "Customer ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Full Name *:", _txtName = new TextBox { Width = 180 });
        AddField(gridFields, 0, 4, "Job Title:", _txtJobTitle = new TextBox { Width = 160 });

        AddField(gridFields, 1, 0, "Phone:", _txtPhone = new TextBox { Width = 90 });
        AddField(gridFields, 1, 2, "Email:", _txtEmail = new TextBox { Width = 180 });
        _cmbMember = new ComboBox { Width = 160 };
        AddField(gridFields, 1, 4, "Member Tier:", _cmbMember);

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

        searchPanel.Children.Add(new TextBlock { Text = "Search Customers:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 350, Watermark = "Filter by name, phone, or email..." };
        _txtSearch.TextChanged += (s, e) => Filter();
        searchPanel.Children.Add(_txtSearch);

        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Avalonia.Data.Binding("CustomerID"), Width = new DataGridLength(60) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Customer Name", Binding = new Avalonia.Data.Binding("CustomerName"), Width = new DataGridLength(180) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Job Title", Binding = new Avalonia.Data.Binding("JobTitle"), Width = new DataGridLength(140) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Phone", Binding = new Avalonia.Data.Binding("Phone"), Width = new DataGridLength(120) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Avalonia.Data.Binding("Email"), Width = new DataGridLength(160) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Member Tier", Binding = new Avalonia.Data.Binding("MemberName"), Width = new DataGridLength(120) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Discount (%)", Binding = new Avalonia.Data.Binding("Discount"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Address", Binding = new Avalonia.Data.Binding("Address"), Width = new DataGridLength(200) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Customer c)
            {
                _txtId.Text = c.CustomerID.ToString();
                _txtName.Text = c.CustomerName;
                _txtJobTitle.Text = c.JobTitle ?? string.Empty;
                _txtPhone.Text = c.Phone ?? string.Empty;
                _txtEmail.Text = c.Email ?? string.Empty;
                _txtAddress.Text = c.Address ?? string.Empty;

                if (c.MemberID.HasValue)
                {
                    var found = _allMembers.FirstOrDefault(m => m.MemberID == c.MemberID.Value);
                    _cmbMember.SelectedItem = found;
                }
                else
                {
                    _cmbMember.SelectedItem = null;
                }
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
            _allMembers = await _memberService.GetAllAsync();
            _cmbMember.ItemsSource = _allMembers;
            _cmbMember.DisplayMemberBinding = new Avalonia.Data.Binding("MemberName");

            _allCustomers = await _service.GetAllAsync();
            _grid.ItemsSource = _allCustomers;
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
            _grid.ItemsSource = _allCustomers;
            return;
        }

        _grid.ItemsSource = _allCustomers.Where(c =>
            c.CustomerName.ToLower().Contains(q) ||
            (c.Phone?.ToLower().Contains(q) ?? false) ||
            (c.Email?.ToLower().Contains(q) ?? false)
        ).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            var selMember = _cmbMember.SelectedItem as Member;
            var c = new Customer
            {
                CustomerName = _txtName.Text?.Trim() ?? string.Empty,
                JobTitle = _txtJobTitle.Text?.Trim(),
                Phone = _txtPhone.Text?.Trim(),
                Email = _txtEmail.Text?.Trim(),
                Address = _txtAddress.Text?.Trim(),
                MemberID = selMember?.MemberID
            };

            int newId = await _service.CreateAsync(c);
            await MessageBoxDialog.ShowAsync(this, $"Customer saved with ID: {newId}", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a customer to update.", "Selection Required");
            return;
        }

        try
        {
            var selMember = _cmbMember.SelectedItem as Member;
            var c = new Customer
            {
                CustomerID = id,
                CustomerName = _txtName.Text?.Trim() ?? string.Empty,
                JobTitle = _txtJobTitle.Text?.Trim(),
                Phone = _txtPhone.Text?.Trim(),
                Email = _txtEmail.Text?.Trim(),
                Address = _txtAddress.Text?.Trim(),
                MemberID = selMember?.MemberID
            };

            await _service.UpdateAsync(c);
            await MessageBoxDialog.ShowAsync(this, "Customer updated.", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a customer to delete.", "Selection Required");
            return;
        }

        if (!await MessageBoxDialog.ShowConfirmAsync(this, $"Delete customer #{id}?", "Confirm")) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Customer deleted.", "Success");
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
        _txtJobTitle.Text = string.Empty;
        _txtPhone.Text = string.Empty;
        _txtEmail.Text = string.Empty;
        _txtAddress.Text = string.Empty;
        _cmbMember.SelectedItem = null;
        _grid.SelectedItem = null;
    }
}
