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

public class PurchaseForm : Window
{
    private readonly PurchaseService _purchaseService;
    private readonly SupplierService _supplierService;
    private readonly ShipperService _shipperService;
    private readonly ProductService _productService;

    private List<Supplier> _suppliers = new();
    private List<Shipper> _shippers = new();
    private List<Product> _products = new();
    private readonly ObservableCollection<PurchaseDetail> _orderItems = new();

    private readonly ComboBox _cmbSupplier;
    private readonly ComboBox _cmbShipper;
    private readonly ComboBox _cmbProduct;
    private readonly TextBox _txtUnitPriceIn;
    private readonly TextBox _txtQty;
    private readonly TextBlock _lblTotal;
    private readonly DataGrid _grid;

    public PurchaseForm()
    {
        _purchaseService = new PurchaseService();
        _supplierService = new SupplierService();
        _shipperService = new ShipperService();
        _productService = new ProductService();

        Title = "Purchase Order Transaction";
        Width = 1080;
        Height = 720;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var root = new DockPanel { Margin = new Thickness(16) };

        // 1. Header Box
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

        topGrid.Children.Add(new TextBlock { Text = "Supplier *:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4) });
        _cmbSupplier = new ComboBox { Width = 220 };
        Grid.SetColumn(_cmbSupplier, 1);
        topGrid.Children.Add(_cmbSupplier);

        var lblShip = new TextBlock { Text = "Shipper:", FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(16, 4, 4, 4) };
        Grid.SetColumn(lblShip, 2);
        topGrid.Children.Add(lblShip);

        _cmbShipper = new ComboBox { Width = 220 };
        Grid.SetColumn(_cmbShipper, 3);
        topGrid.Children.Add(_cmbShipper);

        var lblEmp = new TextBlock
        {
            Text = $"Purchaser: {EmployeeSession.CurrentEmployeeName}",
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.DarkSlateBlue,
            Margin = new Thickness(16, 4, 4, 4)
        };
        Grid.SetColumn(lblEmp, 5);
        topGrid.Children.Add(lblEmp);

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
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto,*"),
            RowDefinitions = new RowDefinitions("Auto")
        };

        _cmbProduct = new ComboBox { Width = 260 };
        _cmbProduct.SelectionChanged += (s, e) => OnProductSelected();
        AddField(prodGrid, 0, 0, "Product *:", _cmbProduct);

        AddField(prodGrid, 0, 2, "Unit Cost ($) *:", _txtUnitPriceIn = new TextBox { Width = 110 });
        AddField(prodGrid, 0, 4, "Purchase Qty *:", _txtQty = new TextBox { Text = "10", Width = 90 });

        var btnAdd = new Button
        {
            Content = "Add to Purchase Order",
            Width = 200,
            HorizontalAlignment = HorizontalAlignment.Right,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnAdd.Click += async (s, e) => await AddToOrderAsync();

        prodStack.Children.Add(prodGrid);
        prodStack.Children.Add(btnAdd);
        prodBox.Child = prodStack;

        // 3. Bottom Summary & Buttons
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
            Text = "Total Cost: $0.00",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.DarkBlue,
            Margin = new Thickness(0, 0, 20, 0)
        };

        var btnSave = new Button { Content = "Save Purchase (Commit to Oracle)", Width = 240, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnSave.Click += async (s, e) => await SavePurchaseAsync();

        var btnRemove = new Button { Content = "Remove Item", Width = 110, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnRemove.Click += (s, e) => RemoveItem();

        var btnClear = new Button { Content = "Clear Order", Width = 100, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClear.Click += (s, e) => ClearOrder();

        var btnClose = new Button { Content = "Close", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        btnClose.Click += (s, e) => Close();

        bottomPanel.Children.Add(_lblTotal);
        bottomPanel.Children.Add(btnSave);
        bottomPanel.Children.Add(btnRemove);
        bottomPanel.Children.Add(btnClear);
        bottomPanel.Children.Add(btnClose);

        // 4. Order Grid
        _grid = new DataGrid
        {
            IsReadOnly = true,
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            GridLinesVisibility = DataGridGridLinesVisibility.All,
            ItemsSource = _orderItems
        };

        _grid.Columns.Add(new DataGridTextColumn { Header = "Product ID", Binding = new Avalonia.Data.Binding("ProductID"), Width = new DataGridLength(100) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Product Name", Binding = new Avalonia.Data.Binding("ProductName"), Width = new DataGridLength(350) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Purchase Qty", Binding = new Avalonia.Data.Binding("PurchaseQty"), Width = new DataGridLength(130) });
        _grid.Columns.Add(new DataGridTextColumn { Header = "Unit Cost ($)", Binding = new Avalonia.Data.Binding("UnitPriceIn"), Width = new DataGridLength(140) });
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
            _suppliers = await _supplierService.GetAllAsync();
            _cmbSupplier.ItemsSource = _suppliers;
            _cmbSupplier.DisplayMemberBinding = new Avalonia.Data.Binding("SupplierName");
            if (_suppliers.Count > 0) _cmbSupplier.SelectedIndex = 0;

            _shippers = await _shipperService.GetAllAsync();
            _cmbShipper.ItemsSource = _shippers;
            _cmbShipper.DisplayMemberBinding = new Avalonia.Data.Binding("CompanyName");
            if (_shippers.Count > 0) _cmbShipper.SelectedIndex = 0;

            _products = await _productService.GetAllAsync(includeDeleted: false);
            _cmbProduct.ItemsSource = _products;
            _cmbProduct.DisplayMemberBinding = new Avalonia.Data.Binding("ProductName");
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Error");
        }
    }

    private void OnProductSelected()
    {
        if (_cmbProduct.SelectedItem is Product p)
        {
            _txtUnitPriceIn.Text = p.UnitPriceIn.ToString("F2");
        }
    }

    private async Task AddToOrderAsync()
    {
        if (_cmbProduct.SelectedItem is not Product p)
        {
            await MessageBoxDialog.ShowAsync(this, "Please select a product.", "Warning");
            return;
        }

        if (!decimal.TryParse(_txtQty.Text, out decimal qty) || qty <= 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Please enter a valid purchase quantity.", "Warning");
            return;
        }

        if (!decimal.TryParse(_txtUnitPriceIn.Text, out decimal priceIn) || priceIn < 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Please enter a valid unit cost price.", "Warning");
            return;
        }

        var existing = _orderItems.FirstOrDefault(item => item.ProductID == p.ProductID);
        if (existing != null)
        {
            existing.PurchaseQty += qty;
            existing.UnitPriceIn = priceIn;
            _orderItems.Remove(existing);
            _orderItems.Add(existing);
        }
        else
        {
            _orderItems.Add(new PurchaseDetail
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                PurchaseQty = qty,
                UnitPriceIn = priceIn
            });
        }

        UpdateTotal();
    }

    private void RemoveItem()
    {
        if (_grid.SelectedItem is PurchaseDetail item)
        {
            _orderItems.Remove(item);
            UpdateTotal();
        }
    }

    private void ClearOrder()
    {
        _orderItems.Clear();
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        decimal total = _orderItems.Sum(item => item.Subtotal);
        _lblTotal.Text = $"Total Cost: ${total:F2}";
    }

    private async Task SavePurchaseAsync()
    {
        if (_orderItems.Count == 0)
        {
            await MessageBoxDialog.ShowAsync(this, "Purchase order is empty. Add products first.", "Empty Order");
            return;
        }

        if (_cmbSupplier.SelectedItem is not Supplier sup)
        {
            await MessageBoxDialog.ShowAsync(this, "Please select a Supplier.", "Supplier Required");
            return;
        }

        var selShip = _cmbShipper.SelectedItem as Shipper;
        int empId = EmployeeSession.CurrentEmployeeID > 0 ? EmployeeSession.CurrentEmployeeID : 1;

        try
        {
            int purId = await _purchaseService.ProcessPurchaseAsync(empId, sup.SupplierID, selShip?.ShipperID, _orderItems.ToList());

            await MessageBoxDialog.ShowAsync(this, $"Purchase Order #{purId} successfully recorded! Inventory increased automatically in Oracle.", "Purchase Saved");
            ClearOrder();
            await LoadFormDataAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Transaction Failed");
        }
    }
}
