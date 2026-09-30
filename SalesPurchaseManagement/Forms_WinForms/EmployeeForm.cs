using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Services;

namespace SalesPurchaseManagement.Forms;

/// <summary>
/// Student-friendly desktop CRUD interface for managing employee records.
/// </summary>
public class EmployeeForm : Form
{
    private readonly EmployeeService _service;
    private List<Employee> _allEmployees = new();

    // Form Controls
    private GroupBox gbEmployeeInfo = null!;
    private TextBox txtEmployeeID = null!;
    private TextBox txtEmployeeName = null!;
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private ComboBox cmbUserType = null!;
    private TextBox txtRole = null!;
    private TextBox txtJobTitle = null!;
    private TextBox txtEmail = null!;
    private TextBox txtPhone = null!;
    private TextBox txtHomePhone = null!;
    private TextBox txtAddress = null!;
    private TextBox txtCity = null!;
    private TextBox txtStateProvince = null!;
    private TextBox txtZipPostalCode = null!;
    private TextBox txtCountryRegion = null!;

    // Action Buttons
    private Button btnNew = null!;
    private Button btnSave = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private Button btnClear = null!;
    private Button btnRefresh = null!;

    // Search and Grid
    private TextBox txtSearch = null!;
    private DataGridView dgvEmployees = null!;

    public EmployeeForm()
    {
        _service = new EmployeeService();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // -------------------------------------------------------------
        // Form Setup
        // -------------------------------------------------------------
        Text = "Employee Management";
        ClientSize = new Size(1020, 680);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        BackColor = SystemColors.Control;
        ForeColor = SystemColors.ControlText;

        // -------------------------------------------------------------
        // GroupBox: Employee Information
        // -------------------------------------------------------------
        gbEmployeeInfo = new GroupBox
        {
            Text = " Employee Information ",
            Location = new Point(15, 12),
            Size = new Size(990, 240),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        };

        // Row 1: ID, Name, Username, Password
        var lblId = CreateLabel("Employee ID:", 20, 25);
        txtEmployeeID = new TextBox { Location = new Point(125, 22), Size = new Size(100, 23), ReadOnly = true, BackColor = Color.WhiteSmoke };

        var lblName = CreateLabel("Full Name *:", 250, 25);
        txtEmployeeName = new TextBox { Location = new Point(350, 22), Size = new Size(180, 23) };

        var lblUser = CreateLabel("Username *:", 550, 25);
        txtUsername = new TextBox { Location = new Point(640, 22), Size = new Size(130, 23) };

        var lblPass = CreateLabel("Password *:", 790, 25);
        txtPassword = new TextBox { Location = new Point(865, 22), Size = new Size(105, 23), UseSystemPasswordChar = true };

        // Row 2: UserType, Role, Job Title, Email
        var lblType = CreateLabel("User Type:", 20, 58);
        cmbUserType = new ComboBox { Location = new Point(125, 55), Size = new Size(100, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbUserType.Items.AddRange(new object[] { "Staff", "Admin" });
        cmbUserType.SelectedIndex = 0;

        var lblRole = CreateLabel("Role:", 250, 58);
        txtRole = new TextBox { Location = new Point(350, 55), Size = new Size(180, 23) };

        var lblTitle = CreateLabel("Job Title:", 550, 58);
        txtJobTitle = new TextBox { Location = new Point(640, 55), Size = new Size(130, 23) };

        var lblEmail = CreateLabel("Email:", 790, 58);
        txtEmail = new TextBox { Location = new Point(865, 55), Size = new Size(105, 23) };

        // Row 3: Phone, Home Phone, City, Country
        var lblPhone = CreateLabel("Phone:", 20, 91);
        txtPhone = new TextBox { Location = new Point(125, 88), Size = new Size(100, 23) };

        var lblHomePhone = CreateLabel("Home Phone:", 250, 91);
        txtHomePhone = new TextBox { Location = new Point(350, 88), Size = new Size(180, 23) };

        var lblCity = CreateLabel("City:", 550, 91);
        txtCity = new TextBox { Location = new Point(640, 88), Size = new Size(130, 23) };

        var lblCountry = CreateLabel("Country:", 790, 91);
        txtCountryRegion = new TextBox { Location = new Point(865, 88), Size = new Size(105, 23), Text = "Cambodia" };

        // Row 4: Address, State/Province, Zip Code
        var lblAddress = CreateLabel("Address:", 20, 124);
        txtAddress = new TextBox { Location = new Point(125, 121), Size = new Size(405, 23) };

        var lblState = CreateLabel("State/Prov:", 550, 124);
        txtStateProvince = new TextBox { Location = new Point(640, 121), Size = new Size(130, 23) };

        var lblZip = CreateLabel("Zip Code:", 790, 124);
        txtZipPostalCode = new TextBox { Location = new Point(865, 121), Size = new Size(105, 23) };

        // Action Buttons Row inside GroupBox
        btnNew = CreateButton("New", 125, 175, (s, e) => ClearForm());
        btnSave = CreateButton("Save", 225, 175, async (s, e) => await SaveEmployeeAsync());
        btnUpdate = CreateButton("Update", 325, 175, async (s, e) => await UpdateEmployeeAsync());
        btnDelete = CreateButton("Delete", 425, 175, async (s, e) => await DeleteEmployeeAsync());
        btnClear = CreateButton("Clear", 525, 175, (s, e) => ClearForm());
        btnRefresh = CreateButton("Refresh", 625, 175, async (s, e) => await LoadEmployeesAsync());

        gbEmployeeInfo.Controls.AddRange(new Control[]
        {
            lblId, txtEmployeeID,
            lblName, txtEmployeeName,
            lblUser, txtUsername,
            lblPass, txtPassword,
            lblType, cmbUserType,
            lblRole, txtRole,
            lblTitle, txtJobTitle,
            lblEmail, txtEmail,
            lblPhone, txtPhone,
            lblHomePhone, txtHomePhone,
            lblCity, txtCity,
            lblCountry, txtCountryRegion,
            lblAddress, txtAddress,
            lblState, txtStateProvince,
            lblZip, txtZipPostalCode,
            btnNew, btnSave, btnUpdate, btnDelete, btnClear, btnRefresh
        });

        // -------------------------------------------------------------
        // Search Panel
        // -------------------------------------------------------------
        var pnlSearch = new Panel
        {
            Location = new Point(15, 260),
            Size = new Size(990, 35)
        };

        var lblSearch = new Label
        {
            Text = "Search Employees:",
            Location = new Point(0, 8),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        };

        txtSearch = new TextBox
        {
            Location = new Point(130, 5),
            Size = new Size(350, 23),
            PlaceholderText = "Type name, username, phone, or email to filter..."
        };
        txtSearch.TextChanged += (s, e) => FilterEmployees();

        pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch });

        // -------------------------------------------------------------
        // DataGridView
        // -------------------------------------------------------------
        dgvEmployees = new DataGridView
        {
            Location = new Point(15, 305),
            Size = new Size(990, 360),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.Fixed3D,
            RowHeadersVisible = false
        };
        dgvEmployees.SelectionChanged += DgvEmployees_SelectionChanged;

        ConfigureGridColumns();

        // -------------------------------------------------------------
        // Form Controls Assembly
        // -------------------------------------------------------------
        Controls.Add(gbEmployeeInfo);
        Controls.Add(pnlSearch);
        Controls.Add(dgvEmployees);

        Load += async (s, e) => await LoadEmployeesAsync();
    }

    private void ConfigureGridColumns()
    {
        dgvEmployees.AutoGenerateColumns = false;
        dgvEmployees.Columns.Clear();

        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EmployeeID", HeaderText = "ID", FillWeight = 35 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EmployeeName", HeaderText = "Employee Name", FillWeight = 110 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserName", HeaderText = "Username", FillWeight = 75 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserType", HeaderText = "User Type", FillWeight = 60 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Role", FillWeight = 80 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "JobTitle", HeaderText = "Job Title", FillWeight = 90 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", FillWeight = 110 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Phone", FillWeight = 75 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "City", HeaderText = "City", FillWeight = 70 });
        dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CountryRegion", HeaderText = "Country", FillWeight = 70 });
    }

    private static Label CreateLabel(string text, int x, int y)
    {
        return new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true
        };
    }

    private static Button CreateButton(string text, int x, int y, EventHandler onClick)
    {
        var btn = new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(88, 30),
            Cursor = Cursors.Hand
        };
        btn.Click += onClick;
        return btn;
    }

    private async Task LoadEmployeesAsync()
    {
        try
        {
            _allEmployees = await _service.GetAllAsync();
            BindGrid(_allEmployees);
        }
        catch (OracleException oex)
        {
            MessageBox.Show($"Oracle Error {oex.Number}: {oex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BindGrid(List<Employee> list)
    {
        dgvEmployees.DataSource = null;
        dgvEmployees.DataSource = list;
    }

    private void FilterEmployees()
    {
        string query = txtSearch.Text.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(query))
        {
            BindGrid(_allEmployees);
            return;
        }

        var filtered = _allEmployees.Where(e =>
            (e.EmployeeName?.ToLower().Contains(query) ?? false) ||
            (e.UserName?.ToLower().Contains(query) ?? false) ||
            (e.Phone?.ToLower().Contains(query) ?? false) ||
            (e.Email?.ToLower().Contains(query) ?? false) ||
            (e.Role?.ToLower().Contains(query) ?? false)
        ).ToList();

        BindGrid(filtered);
    }

    private void DgvEmployees_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvEmployees.SelectedRows.Count == 0) return;

        var selected = dgvEmployees.SelectedRows[0].DataBoundItem as Employee;
        if (selected == null) return;

        txtEmployeeID.Text = selected.EmployeeID.ToString();
        txtEmployeeName.Text = selected.EmployeeName;
        txtUsername.Text = selected.UserName;
        txtPassword.Text = string.Empty; // Never display password from DB
        cmbUserType.SelectedItem = selected.UserType ?? "Staff";
        txtRole.Text = selected.Role ?? string.Empty;
        txtJobTitle.Text = selected.JobTitle ?? string.Empty;
        txtEmail.Text = selected.Email ?? string.Empty;
        txtPhone.Text = selected.Phone ?? string.Empty;
        txtHomePhone.Text = selected.HomePhone ?? string.Empty;
        txtAddress.Text = selected.Address ?? string.Empty;
        txtCity.Text = selected.City ?? string.Empty;
        txtStateProvince.Text = selected.StateProvince ?? string.Empty;
        txtZipPostalCode.Text = selected.ZipPostalCode ?? string.Empty;
        txtCountryRegion.Text = selected.CountryRegion ?? string.Empty;
    }

    private async Task SaveEmployeeAsync()
    {
        try
        {
            var employee = BuildEmployeeFromForm();

            int newId = await _service.CreateAsync(employee);
            MessageBox.Show($"Employee saved successfully with ID: {newId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await LoadEmployeesAsync();
            ClearForm();
        }
        catch (ArgumentException aex)
        {
            MessageBox.Show(aex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (InvalidOperationException iex)
        {
            MessageBox.Show(iex.Message, "Business Rule Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (OracleException oex)
        {
            MessageBox.Show($"Oracle Error {oex.Number}: {oex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unexpected error: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task UpdateEmployeeAsync()
    {
        if (string.IsNullOrWhiteSpace(txtEmployeeID.Text) || !int.TryParse(txtEmployeeID.Text, out int id) || id <= 0)
        {
            MessageBox.Show("Please select an employee from the table to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var employee = BuildEmployeeFromForm();
            employee.EmployeeID = id;

            await _service.UpdateAsync(employee);
            MessageBox.Show("Employee updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await LoadEmployeesAsync();
        }
        catch (ArgumentException aex)
        {
            MessageBox.Show(aex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (InvalidOperationException iex)
        {
            MessageBox.Show(iex.Message, "Business Rule Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (OracleException oex)
        {
            MessageBox.Show($"Oracle Error {oex.Number}: {oex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unexpected error: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task DeleteEmployeeAsync()
    {
        if (string.IsNullOrWhiteSpace(txtEmployeeID.Text) || !int.TryParse(txtEmployeeID.Text, out int id) || id <= 0)
        {
            MessageBox.Show("Please select an employee from the table to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Are you sure you want to delete employee #{id} ({txtEmployeeName.Text})?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            await _service.DeleteAsync(id);
            MessageBox.Show("Employee deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await LoadEmployeesAsync();
            ClearForm();
        }
        catch (InvalidOperationException iex)
        {
            MessageBox.Show(iex.Message, "Deletion Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (OracleException oex)
        {
            MessageBox.Show($"Oracle Error {oex.Number}: {oex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unexpected error: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Employee BuildEmployeeFromForm()
    {
        return new Employee
        {
            EmployeeName = txtEmployeeName.Text.Trim(),
            UserName = txtUsername.Text.Trim(),
            Password = string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text.Trim(),
            UserType = cmbUserType.SelectedItem?.ToString() ?? "Staff",
            Role = string.IsNullOrWhiteSpace(txtRole.Text) ? null : txtRole.Text.Trim(),
            JobTitle = string.IsNullOrWhiteSpace(txtJobTitle.Text) ? null : txtJobTitle.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
            Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
            HomePhone = string.IsNullOrWhiteSpace(txtHomePhone.Text) ? null : txtHomePhone.Text.Trim(),
            Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
            City = string.IsNullOrWhiteSpace(txtCity.Text) ? null : txtCity.Text.Trim(),
            StateProvince = string.IsNullOrWhiteSpace(txtStateProvince.Text) ? null : txtStateProvince.Text.Trim(),
            ZipPostalCode = string.IsNullOrWhiteSpace(txtZipPostalCode.Text) ? null : txtZipPostalCode.Text.Trim(),
            CountryRegion = string.IsNullOrWhiteSpace(txtCountryRegion.Text) ? null : txtCountryRegion.Text.Trim()
        };
    }

    private void ClearForm()
    {
        txtEmployeeID.Clear();
        txtEmployeeName.Clear();
        txtUsername.Clear();
        txtPassword.Clear();
        cmbUserType.SelectedIndex = 0;
        txtRole.Clear();
        txtJobTitle.Clear();
        txtEmail.Clear();
        txtPhone.Clear();
        txtHomePhone.Clear();
        txtAddress.Clear();
        txtCity.Clear();
        txtStateProvince.Clear();
        txtZipPostalCode.Clear();
        txtCountryRegion.Text = "Cambodia";

        dgvEmployees.ClearSelection();
        txtEmployeeName.Focus();
    }
}
