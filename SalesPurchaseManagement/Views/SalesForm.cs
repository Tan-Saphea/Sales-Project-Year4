using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

public class SalesForm : Window
{
    private readonly SalesService _salesService;
    private readonly CustomerService _customerService;
    private readonly ProductService _productService;

    private List<Customer> _customers = new();
    private List<Product> _products = new();
    private readonly ObservableCollection<SalesDetail> _cartItems = new();

    private readonly ComboBox _cmbCustomer;
    private readonly ComboBox _cmbProduct;
    private readonly TextBox _txtBarcode;
    private readonly TextBox _txtAvailQty;
    private readonly TextBox _txtPrice;
    private readonly TextBox _txtSaleQty;
    private readonly TextBlock _lblTotal;
    private readonly DataGrid _grid;

    public SalesForm()
    {
        _salesService = new SalesService();
        _customerService = new CustomerService();
        _productService = new ProductService();

        Title = "Sales Invoice (POS Transaction)";
        Width = 1080;
        Height = 720;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var root = new DockPanel { Margin = new Thickness(16) };

        // 1. Header Box: Employee and Customer
        var topBox = new Border
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 0, 0, 10)
        };
        DockPanel.SetDock(topBox, Dock.Top);

        var topGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto")
        };

        topGrid.Children.Add(new TextBlock { Text = "Cashier:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4) });
        var txtCashier = new TextBlock
        {
            Text = $"{EmployeeSession.CurrentEmployeeName} (ID: {EmployeeSession.CurrentEmployeeID})",
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.DarkSlateBlue
        };
        Grid.SetColumn(txtCashier, 1);
        topGrid.Children.Add(txtCashier);

        var lblCust = new TextBlock { Text = "Customer:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(16, 4, 4, 4) };
        Grid.SetColumn(lblCust, 2);
        topGrid.Children.Add(lblCust);

        _cmbCustomer = new ComboBox { Width = 220 };
        Grid.SetColumn(_cmbCustomer, 3);
        topGrid.Children.Add(_cmbCustomer);

        var lblDate = new TextBlock { Text = $"Date: {DateTime.Now:yyyy-MM-dd HH:mm}", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(16, 4, 4, 4) };
        Grid.SetColumn(lblDate, 5);
        topGrid.Children.Add(lblDate);

        topBox.Child = topGrid;

        // 2. Product Picker Box
        var prodBox = new Border
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 0, 0, 10)
        };
        DockPanel.SetDock(prodBox, Dock.Top);

        var prodStack = new StackPanel { Spacing = 10 };

        var prodGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto,*,Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto")
        };

        AddField(prodGrid, 0, 0, "Barcode:", _txtBarcode = new TextBox { Width = 110 });
        _txtBarcode.KeyDown += (s, e) => { if (e.Key == Avalonia.Input.Key.Enter) LookupBarcode(); };

        _cmbProduct = new ComboBox { Width = 220 };
        _cmbProduct.SelectionChanged += (s, e) => OnProductSelected();
        AddField(prodGrid, 0, 2, "Product:", _cmbProduct);

        AddField(prodGrid, 0, 4, "Price ($):", _txtPrice = new TextBox { IsReadOnly = true, Width = 80 });
        AddField(prodGrid, 0, 6, "In Stock:", _txtAvailQty = new TextBox { IsReadOnly = true, Width = 70 });
        AddField(prodGrid, 0, 8, "Sale Qty:", _txtSaleQty = new TextBox { Text = "1", Width = 70 });

        var btnAdd = new Button
        {
            Content = "Add Product to Cart",
            Width = 180,
            HorizontalAlignment = HorizontalAlignment.Right,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnAdd.Click += async (s, e) => await AddToCartAsync();

        prodStack.Children.Add(prodGrid);
        prodStack.Children.Add(btnAdd);
        prodBox.Child = prodStack;

        // 3. Bottom Summary & Action Buttons
        var bottomPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 16,
            Margin = new Thickness(0, 10, 0, 0)
        };
        DockPanel.SetDock(bottomPanel, Dock.Bottom);

        _lblTotal = new TextBlock
        {
            Text = "Total: $0.00",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.ForestGreen,
            Margin = new Thickness(0, 0, 20, 0)
        };

        var btnSave = new Button { Content = "Save Sale (Commit to Oracle)", Width = 220, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnSave.Click += async (s, e) => await SaveSaleAsync();

        var btnRemove = new Button { Content = "Remove Item", Width = 110, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnRemove.Click += (s, e) => RemoveItem();

        var btnClear = new Button { Content = "Clear Cart", Width = 90, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClear.Click += (s, e) => ClearCart();

        var btnClose = new Button { Content = "Close", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClose.Click += (s, e) => Close();

        bottomPanel.Children.Add(_lblTotal);
        bottomPanel.Children.Add(btnSave);
        bottomPanel.Children.Add(btnRemove);
        bottomPanel.Children.Add(btnClear);
        bottomPanel.Children.Add(btnClose);

        // 4. Cart DataGrid
        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All,
            ItemsSource = _cartItems
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "Product ID", Binding = new Avalonia.Data.Binding("ProductID"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Product Name", Binding = new Avalonia.Data.Binding("ProductName"), Width = new DataGridLength(350) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Qty", Binding = new Avalonia.Data.Binding("QtySales"), Width = new DataGridLength(120) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Unit Price ($)", Binding = new Avalonia.Data.Binding("UnitPriceOut"), Width = new DataGridLength(140) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Subtotal ($)", Binding = new Avalonia.Data.Binding("Subtotal"), Width = new DataGridLength(140) });

        root.Children.Add(topBox);
        root.Children.Add(prodBox);
        root.Children.Add(bottomPanel);
        root.Children.Add(_grid);

        Content = root;
        Loaded += async (s, e) => await LoadFormDataAsync();
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

    private async Task LoadFormDataAsync()
    {
        try
        {
            _customers = await _customerService.GetAllAsync();
            _cmbCustomer.ItemsSource = _customers;
            _cmbCustomer.DisplayMemberBinding = new Avalonia.Data.Binding("CustomerName");
            if (_customers.Count > 0) _cmbCustomer.SelectedIndex = 0;

            _products = await _productService.GetAllAsync(includeDeleted: false);
            _cmbProduct.ItemsSource = _products;
            _cmbProduct.DisplayMemberBinding = new Avalonia.Data.Binding("ProductName");
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error Loading Form Data");
        }
    }

    private void OnProductSelected()
    {
        if (_cmbProduct.SelectedItem is Product p)
        {
            _txtBarcode.Text = p.Barcode ?? string.Empty;
            _txtPrice.Text = p.UnitPriceOut.ToString("F2");
            _txtAvailQty.Text = p.Qty.ToString();
        }
    }

    private void LookupBarcode()
    {
        string bc = _txtBarcode.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(bc)) return;

        var found = _products.FirstOrDefault(p => string.Equals(p.Barcode, bc, StringComparison.OrdinalIgnoreCase));
        if (found != null)
        {
            _cmbProduct.SelectedItem = found;
        }
    }

    private async Task AddToCartAsync()
    {
        if (_cmbProduct.SelectedItem is not Product p)
        {
            await MessageBoxDialog.ShowAsync(this, "Please select a product.", "Warning");
            return;
        }

        if (!decimal.TryParse(_txtSaleQty.Text, out decimal qty) || qty <= 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Please enter a valid positive sale quantity.", "Warning");
            return;
        }

        if (qty > p.Qty)
        {
            await MessageBoxDialog.ShowAsync(this, $"Insufficient stock. Available: {p.Qty}", "Stock Error");
            return;
        }

        var existing = _cartItems.FirstOrDefault(item => item.ProductID == p.ProductID);
        if (existing != null)
        {
            if (existing.QtySales + qty > p.Qty)
            {
                await MessageBoxDialog.ShowAsync(this, $"Total quantity in cart would exceed stock ({p.Qty}).", "Stock Error");
                return;
            }
            existing.QtySales += qty;
            _cartItems.Remove(existing);
            _cartItems.Add(existing);
        }
        else
        {
            _cartItems.Add(new SalesDetail
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                QtySales = qty,
                UnitPriceOut = p.UnitPriceOut
            });
        }

        UpdateTotal();
    }

    private void RemoveItem()
    {
        if (_grid.SelectedItem is SalesDetail item)
        {
            _cartItems.Remove(item);
            UpdateTotal();
        }
    }

    private void ClearCart()
    {
        _cartItems.Clear();
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        decimal total = _cartItems.Sum(item => item.Subtotal);
        _lblTotal.Text = $"Total: ${total:F2}";
    }

    private async Task SaveSaleAsync()
    {
        if (_cartItems.Count == 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Cannot save an empty sale. Add products first.", "Empty Cart");
            return;
        }

        var selCust = _cmbCustomer.SelectedItem as Customer;
        int empId = EmployeeSession.CurrentEmployeeID > 0 ? EmployeeSession.CurrentEmployeeID : 1;

        try
        {
            int salesId = await _salesService.ProcessSaleAsync(empId, selCust?.CustomerID, _cartItems.ToList());

            await MessageBoxDialog.ShowAsync(this, $"Sale #{salesId} completed successfully! Stock decreased automatically in Oracle.", "Sale Recorded");
            ClearCart();
            await LoadFormDataAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Transaction Failed");
        }
    }
}
