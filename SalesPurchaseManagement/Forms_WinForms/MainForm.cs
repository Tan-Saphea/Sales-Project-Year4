using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SalesPurchaseManagement.Data;

namespace SalesPurchaseManagement.Forms;

/// <summary>
/// Main application window with clean student-oriented desktop layout,
/// standard MenuStrip navigation, and database connection diagnostics.
/// </summary>
public class MainForm : Form
{
    private readonly OracleDbConnection _oracleDbConnection;

    // UI Controls
    private MenuStrip _menuStrip = null!;
    private Panel _centerPanel = null!;
    private GroupBox _gbSystemInfo = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private Label _lblDbEngine = null!;
    private Label _lblDbHost = null!;
    private Label _lblDbUser = null!;
    private Label _lblCurrentUser = null!;
    private Button _btnTestConnection = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _tssStatus = null!;

    public MainForm()
    {
        _oracleDbConnection = new OracleDbConnection();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // -------------------------------------------------------------
        // Form Properties
        // -------------------------------------------------------------
        Text = "Sales & Purchase Management System";
        ClientSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        BackColor = SystemColors.Control;
        ForeColor = SystemColors.ControlText;

        // -------------------------------------------------------------
        // MenuStrip
        // -------------------------------------------------------------
        _menuStrip = new MenuStrip
        {
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        };

        // File Menu
        var fileMenu = new ToolStripMenuItem("&File");
        var miLogout = new ToolStripMenuItem("&Logout", null, (s, e) => MessageBox.Show("Session logged out.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information));
        var miExit = new ToolStripMenuItem("E&xit", null, (s, e) => Application.Exit());
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { miLogout, new ToolStripSeparator(), miExit });

        // Management Menu
        var mgmtMenu = new ToolStripMenuItem("&Management");
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("&Employees", null, (s, e) => OpenEmployeeForm()));
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("&Members", null, (s, e) => ShowNotImplemented("Members")));
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("&Customers", null, (s, e) => ShowNotImplemented("Customers")));
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("&Categories", null, (s, e) => ShowNotImplemented("Categories")));
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("&Products", null, (s, e) => ShowNotImplemented("Products")));
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("&Suppliers", null, (s, e) => ShowNotImplemented("Suppliers")));
        mgmtMenu.DropDownItems.Add(new ToolStripMenuItem("S&hippers", null, (s, e) => ShowNotImplemented("Shippers")));

        // Transactions Menu
        var transMenu = new ToolStripMenuItem("&Transactions");
        transMenu.DropDownItems.Add(new ToolStripMenuItem("&Sales Invoice", null, (s, e) => ShowNotImplemented("Sales")));
        transMenu.DropDownItems.Add(new ToolStripMenuItem("&Purchase Order", null, (s, e) => ShowNotImplemented("Purchases")));

        // Database Menu
        var dbMenu = new ToolStripMenuItem("&Database");
        var miTestConn = new ToolStripMenuItem("&Test Connection", null, async (s, e) => await RunConnectionTestAsync());
        dbMenu.DropDownItems.Add(miTestConn);

        // Help Menu
        var helpMenu = new ToolStripMenuItem("&Help");
        var miAbout = new ToolStripMenuItem("&About", null, (s, e) => MessageBox.Show(
            "Sales & Purchase Management System\nVersion 1.0\nDatabase: Oracle Database (FREEPDB1)\nBuilt with C# & .NET 8 WinForms",
            "About Application",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information));
        helpMenu.DropDownItems.Add(miAbout);

        _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, mgmtMenu, transMenu, dbMenu, helpMenu });

        // -------------------------------------------------------------
        // StatusStrip
        // -------------------------------------------------------------
        _statusStrip = new StatusStrip();
        _tssStatus = new ToolStripStatusLabel
        {
            Text = "Ready | Oracle FREEPDB1: localhost:1521 | SALESPURCHASEDB"
        };
        _statusStrip.Items.Add(_tssStatus);

        // -------------------------------------------------------------
        // Center Content Panel
        // -------------------------------------------------------------
        _centerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(40)
        };

        _lblTitle = new Label
        {
            Text = "Sales & Purchase Management System",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point),
            Location = new Point(40, 30),
            AutoSize = true
        };

        _lblSubtitle = new Label
        {
            Text = "University Project — Oracle Database Management System",
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.DimGray,
            Location = new Point(42, 68),
            AutoSize = true
        };

        // GroupBox: Database & Session Details
        _gbSystemInfo = new GroupBox
        {
            Text = " Database & System Information ",
            Location = new Point(40, 110),
            Size = new Size(650, 260),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point)
        };

        _lblDbEngine = new Label
        {
            Text = "Database Engine:  Oracle Database (FREEPDB1)",
            Location = new Point(30, 40),
            AutoSize = true
        };

        _lblDbHost = new Label
        {
            Text = "Host / Service:    localhost:1521 / FREEPDB1",
            Location = new Point(30, 75),
            AutoSize = true
        };

        _lblDbUser = new Label
        {
            Text = "Schema User:      SALESPURCHASEDB",
            Location = new Point(30, 110),
            AutoSize = true
        };

        _lblCurrentUser = new Label
        {
            Text = "Logged In User:   Current Employee (Administrator)",
            Location = new Point(30, 145),
            AutoSize = true
        };

        _btnTestConnection = new Button
        {
            Text = "Test Oracle Connection",
            Location = new Point(30, 190),
            Size = new Size(200, 36),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };
        _btnTestConnection.Click += async (s, e) => await RunConnectionTestAsync();

        _gbSystemInfo.Controls.AddRange(new Control[]
        {
            _lblDbEngine,
            _lblDbHost,
            _lblDbUser,
            _lblCurrentUser,
            _btnTestConnection
        });

        _centerPanel.Controls.AddRange(new Control[]
        {
            _lblTitle,
            _lblSubtitle,
            _gbSystemInfo
        });

        // -------------------------------------------------------------
        // Add Top-Level Controls
        // -------------------------------------------------------------
        Controls.Add(_centerPanel);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;
    }

    /// <summary>
    /// Executes the Oracle database connection test and displays results in a standard dialog.
    /// </summary>
    private async Task RunConnectionTestAsync()
    {
        _tssStatus.Text = "Testing Oracle database connection...";
        _btnTestConnection.Enabled = false;

        try
        {
            string result = await _oracleDbConnection.TestConnectionAsync();

            if (result.Contains("Successful"))
            {
                MessageBox.Show(
                    result,
                    "Database Connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                _tssStatus.Text = "Connection verified: SALESPURCHASEDB @ localhost:1521/FREEPDB1";
            }
            else
            {
                MessageBox.Show(
                    result,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                _tssStatus.Text = "Database connection failed.";
            }
        }
        finally
        {
            _btnTestConnection.Enabled = true;
        }
    }

    private void OpenEmployeeForm()
    {
        using var form = new EmployeeForm();
        form.ShowDialog();
    }

    private void ShowNotImplemented(string moduleName)
    {
        MessageBox.Show(
            $"{moduleName} form will be implemented in subsequent steps after connection verification.",
            "Module Notice",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
