using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using SalesPurchaseManagement.Data;
using SalesPurchaseManagement.Security;
using SalesPurchaseManagement.Views.Dialogs;

namespace SalesPurchaseManagement.Views;

/// <summary>
/// Classic C# desktop main form matching the student project layout:
/// Top ToolStrip with icon+text buttons, gray MDI workspace, and bottom status strip.
/// </summary>
public class MainForm : Window
{
    private readonly OracleDbConnection _oracleDbConnection;

    // Status bar labels
    private readonly TextBlock _lblUserNameVal;
    private readonly TextBlock _lblUserTypeVal;
    private readonly TextBlock _lblClock;
    private readonly DispatcherTimer _clockTimer;

    public MainForm()
    {
        _oracleDbConnection = new OracleDbConnection();

        Title = "Sales Management System [Main Form]";
        Width = 1080;
        Height = 680;
        MinWidth = 880;
        MinHeight = 550;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = SolidColorBrush.Parse("#9CA3AF");

        var rootDock = new DockPanel();

        // -------------------------------------------------------------
        // 1. Top ToolStrip Bar (White background, horizontal buttons)
        // -------------------------------------------------------------
        var toolStrip = new Border
        {
            Background = SolidColorBrush.Parse("#FFFFFF"),
            BorderBrush = SolidColorBrush.Parse("#CBD5E1"),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(8, 4)
        };
        DockPanel.SetDock(toolStrip, Dock.Top);

        var toolStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 4
        };

        // Standard icons & actions
        toolStack.Children.Add(CreateNavButton("Employees", "M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z", () => OpenForm(new EmployeeForm())));
        toolStack.Children.Add(CreateNavButton("Suppliers", "M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66 1.34 3 3 3s3-1.34 3-3h6c0 1.66 1.34 3 3 3s3-1.34 3-3h2v-5l-3-4zM6 18.5c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm13.5-9l1.96 2.5H17V9.5h2.5zm-1.5 9c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z", () => OpenForm(new SupplierForm())));
        toolStack.Children.Add(CreateNavButton("Products", "M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-5 14H7v-2h7v2zm3-4H7v-2h10v2zm0-4H7V7h10v2z", () => OpenForm(new ProductForm())));
        toolStack.Children.Add(CreateNavButton("Categories", "M10 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2h-8l-2-2z", () => OpenForm(new CategoryForm())));
        toolStack.Children.Add(CreateNavButton("Sales", "M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l.03-.12.9-1.63h7.45c.75 0 1.41-.41 1.75-1.03l3.58-6.49c.08-.14.12-.31.12-.48 0-.55-.45-1-1-1H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-.9-2-2-2z", () => OpenForm(new SalesForm())));
        toolStack.Children.Add(CreateNavButton("Customers", "M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z", () => OpenForm(new CustomerForm())));
        toolStack.Children.Add(CreateNavButton("Members", "M20 4H4c-1.11 0-1.99.89-1.99 2L2 18c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V6c0-1.11-.89-2-2-2zm0 14H4v-6h16v6zm0-10H4V6h16v2z", () => OpenForm(new MemberForm())));
        toolStack.Children.Add(CreateNavButton("Purchases", "M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM11 10h2v3h3v2h-3v3h-2v-3H8v-2h3z", () => OpenForm(new PurchaseForm())));

        // "More..." Dropdown Menu Button
        toolStack.Children.Add(CreateMoreDropdownButton());

        toolStrip.Child = toolStack;
        rootDock.Children.Add(toolStrip);

        // -------------------------------------------------------------
        // 2. Bottom StatusStrip (User Name: dara  User Type: Admin  Date/Time)
        // -------------------------------------------------------------
        var statusStrip = new Border
        {
            Background = SolidColorBrush.Parse("#F8FAFC"),
            BorderBrush = SolidColorBrush.Parse("#CBD5E1"),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(14, 5)
        };
        DockPanel.SetDock(statusStrip, Dock.Bottom);

        var statusStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            VerticalAlignment = VerticalAlignment.Center
        };

        statusStack.Children.Add(new TextBlock
        {
            Text = "User Name:",
            FontSize = 12,
            Foreground = SolidColorBrush.Parse("#1E293B"),
            VerticalAlignment = VerticalAlignment.Center
        });

        _lblUserNameVal = new TextBlock
        {
            Text = EmployeeSession.CurrentUserName,
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            Foreground = SolidColorBrush.Parse("#16A34A"),
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        statusStack.Children.Add(_lblUserNameVal);

        statusStack.Children.Add(new TextBlock
        {
            Text = "User Type:",
            FontSize = 12,
            Foreground = SolidColorBrush.Parse("#1E293B"),
            VerticalAlignment = VerticalAlignment.Center
        });

        _lblUserTypeVal = new TextBlock
        {
            Text = EmployeeSession.CurrentRole,
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            Foreground = SolidColorBrush.Parse("#DC2626"),
            Margin = new Thickness(0, 0, 14, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        statusStack.Children.Add(_lblUserTypeVal);

        _lblClock = new TextBlock
        {
            Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy h:mm:ss tt"),
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = SolidColorBrush.Parse("#1D4ED8"),
            VerticalAlignment = VerticalAlignment.Center
        };
        statusStack.Children.Add(_lblClock);

        statusStrip.Child = statusStack;
        rootDock.Children.Add(statusStrip);

        // -------------------------------------------------------------
        // 3. Center Workspace (Classic MDI Gray Canvas)
        // -------------------------------------------------------------
        var centerWorkspace = new Panel
        {
            Background = SolidColorBrush.Parse("#9CA3AF")
        };
        rootDock.Children.Add(centerWorkspace);

        Content = rootDock;

        // Clock timer updating every 1 second
        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _clockTimer.Tick += (s, e) =>
        {
            _lblClock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy h:mm:ss tt");
        };
        _clockTimer.Start();

        Loaded += (s, e) => RefreshSessionInfo();
    }

    // -------------------------------------------------------------
    // Navigation Buttons Construction
    // -------------------------------------------------------------
    private static Button CreateNavButton(string text, string pathData, Action onClick)
    {
        var btn = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(10, 8),
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };

        var stack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            VerticalAlignment = VerticalAlignment.Center
        };

        var icon = new Avalonia.Controls.Shapes.Path
        {
            Data = StreamGeometry.Parse(pathData),
            Fill = SolidColorBrush.Parse("#008080"),
            Width = 18,
            Height = 18,
            Stretch = Stretch.Uniform,
            VerticalAlignment = VerticalAlignment.Center
        };

        var label = new TextBlock
        {
            Text = text,
            FontSize = 13,
            FontWeight = FontWeight.Normal,
            Foreground = SolidColorBrush.Parse("#1E293B"),
            VerticalAlignment = VerticalAlignment.Center
        };

        stack.Children.Add(icon);
        stack.Children.Add(label);
        btn.Content = stack;
        btn.Click += (s, e) => onClick();
        return btn;
    }

    private Button CreateMoreDropdownButton()
    {
        var contextMenu = new ContextMenu();

        var miReports = new MenuItem { Header = "Reports" };
        miReports.Click += async (s, e) => await MessageBoxDialog.ShowAsync(this,
            "Sales & Purchase Summary Reports:\n- Daily Sales Summary\n- Low Stock Alerts\n- Supplier Purchase Logs\n\nDatabase: Oracle Database 23ai (FREEPDB1)",
            "System Reports");

        var miShippers = new MenuItem { Header = "Shippers" };
        miShippers.Click += (s, e) => OpenForm(new ShipperForm());

        var miChangePass = new MenuItem { Header = "Change Password" };
        miChangePass.Click += async (s, e) => await MessageBoxDialog.ShowAsync(this,
            $"Password management for user '{EmployeeSession.CurrentUserName}' is active.",
            "Change Password");

        var miTestDb = new MenuItem { Header = "Database Connection" };
        miTestDb.Click += async (s, e) => await TestConnectionAsync();

        var miLogout = new MenuItem { Header = "Logout" };
        miLogout.Click += (s, e) => Logout();

        contextMenu.ItemsSource = new Control[]
        {
            miReports,
            miShippers,
            miChangePass,
            miTestDb,
            new Separator(),
            miLogout
        };

        var btnMore = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(10, 8),
            ContextMenu = contextMenu,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };

        var stack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            VerticalAlignment = VerticalAlignment.Center
        };

        var icon = new Avalonia.Controls.Shapes.Path
        {
            Data = StreamGeometry.Parse("M20 6h-8l-2-2H4c-1.11 0-1.99.89-1.99 2L2 18c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V8c0-1.11-.89-2-2-2zm-1 8h-3v3h-2v-3h-3v-2h3V9h2v3h3v2z"),
            Fill = SolidColorBrush.Parse("#008080"),
            Width = 18,
            Height = 18,
            Stretch = Stretch.Uniform,
            VerticalAlignment = VerticalAlignment.Center
        };

        var label = new TextBlock
        {
            Text = "More...",
            FontSize = 13,
            FontWeight = FontWeight.Normal,
            Foreground = SolidColorBrush.Parse("#1E293B"),
            VerticalAlignment = VerticalAlignment.Center
        };

        stack.Children.Add(icon);
        stack.Children.Add(label);
        btnMore.Content = stack;
        btnMore.Click += (s, e) => contextMenu.Open(btnMore);

        return btnMore;
    }

    public void RefreshSessionInfo()
    {
        _lblUserNameVal.Text = EmployeeSession.CurrentUserName;
        _lblUserTypeVal.Text = EmployeeSession.CurrentRole;
        _lblClock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy h:mm:ss tt");
    }

    private async System.Threading.Tasks.Task TestConnectionAsync()
    {
        try
        {
            string result = await _oracleDbConnection.TestConnectionAsync();
            await MessageBoxDialog.ShowAsync(this, result, "Oracle Connectivity Test");
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Database Error");
        }
    }

    private void OpenForm(Window window)
    {
        window.ShowDialog(this);
    }

    private void Logout()
    {
        _clockTimer.Stop();
        EmployeeSession.ClearSession();
        Hide();
        var login = new LoginForm(session =>
        {
            RefreshSessionInfo();
            _clockTimer.Start();
            Show();
        });
        login.Show();
    }
}
