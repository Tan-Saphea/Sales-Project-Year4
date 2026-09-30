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

public class MemberForm : Window
{
    private readonly MemberService _service;
    private List<Member> _allMembers = new();

    private readonly TextBox _txtId;
    private readonly TextBox _txtName;
    private readonly TextBox _txtDiscount;
    private readonly TextBox _txtSearch;
    private readonly DataGrid _grid;

    public MemberForm()
    {
        _service = new MemberService();

        Title = "Member Tier Management";
        Width = 850;
        Height = 580;
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
            RowDefinitions = new RowDefinitions("Auto")
        };

        AddField(gridFields, 0, 0, "Member ID:", _txtId = new TextBox { IsReadOnly = true, Width = 90 });
        AddField(gridFields, 0, 2, "Member Name *:", _txtName = new TextBox { Width = 180 });
        AddField(gridFields, 0, 4, "Discount (%):", _txtDiscount = new TextBox { Text = "0", Width = 90 });

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

        searchPanel.Children.Add(new TextBlock { Text = "Search Members:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        _txtSearch = new TextBox { Width = 300, Watermark = "Filter member name..." };
        _txtSearch.TextChanged += (s, e) => Filter();
        searchPanel.Children.Add(_txtSearch);

        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "Member ID", Binding = new Avalonia.Data.Binding("MemberID"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Member Name", Binding = new Avalonia.Data.Binding("MemberName"), Width = new DataGridLength(250) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Discount (%)", Binding = new Avalonia.Data.Binding("Discount"), Width = new DataGridLength(120) });

        _grid.SelectionChanged += (s, e) =>
        {
            if (_grid.SelectedItem is Member m)
            {
                _txtId.Text = m.MemberID.ToString();
                _txtName.Text = m.MemberName;
                _txtDiscount.Text = m.Discount.ToString();
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
            _allMembers = await _service.GetAllAsync();
            _grid.ItemsSource = _allMembers;
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
            _grid.ItemsSource = _allMembers;
            return;
        }
        _grid.ItemsSource = _allMembers.Where(m => m.MemberName.ToLower().Contains(q)).ToList();
    }

    private async Task SaveAsync()
    {
        try
        {
            decimal.TryParse(_txtDiscount.Text, out decimal discount);
            var m = new Member { MemberName = _txtName.Text?.Trim() ?? string.Empty, Discount = discount };
            int newId = await _service.CreateAsync(m);
            await MessageBoxDialog.ShowAsync(this, $"Member saved with ID: {newId}", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a member to update.", "Selection Required");
            return;
        }

        try
        {
            decimal.TryParse(_txtDiscount.Text, out decimal discount);
            var m = new Member { MemberID = id, MemberName = _txtName.Text?.Trim() ?? string.Empty, Discount = discount };
            await _service.UpdateAsync(m);
            await MessageBoxDialog.ShowAsync(this, "Member updated.", "Success");
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
            await MessageBoxDialog.ShowAsync(this, "Select a member to delete.", "Selection Required");
            return;
        }

        if (!await MessageBoxDialog.ShowConfirmAsync(this, $"Delete member #{id}?", "Confirm")) return;

        try
        {
            await _service.DeleteAsync(id);
            await MessageBoxDialog.ShowAsync(this, "Member deleted.", "Success");
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
        _txtDiscount.Text = "0";
        _grid.SelectedItem = null;
    }
}
