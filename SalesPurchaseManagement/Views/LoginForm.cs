using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using SalesPurchaseManagement.Security;
using SalesPurchaseManagement.Services;
using SalesPurchaseManagement.Views.Dialogs;

namespace SalesPurchaseManagement.Views;

public class LoginForm : Window
{
    private readonly AuthService _authService;
    private readonly TextBox _txtUsername;
    private readonly TextBox _txtPassword;
    private readonly Action<EmployeeSessionState> _onLoginSuccess;

    public record EmployeeSessionState(int EmployeeID, string Name, string Role);

    public LoginForm(Action<EmployeeSessionState> onLoginSuccess)
    {
        _authService = new AuthService();
        _onLoginSuccess = onLoginSuccess;

        Title = "Employee Login";
        Width = 440;
        Height = 310;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var mainPanel = new StackPanel
        {
            Margin = new Thickness(36, 24),
            Spacing = 14
        };

        var lblHeader = new TextBlock
        {
            Text = "Sales & Purchase Management",
            FontSize = 17,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var lblSub = new TextBlock
        {
            Text = "Employee Authentication (Oracle Database)",
            FontSize = 11,
            Foreground = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var userPanel = new StackPanel { Spacing = 4 };
        userPanel.Children.Add(new TextBlock { Text = "Username:", FontSize = 12 });
        _txtUsername = new TextBox { Text = "dara", Watermark = "Enter username" };
        userPanel.Children.Add(_txtUsername);

        var passPanel = new StackPanel { Spacing = 4 };
        passPanel.Children.Add(new TextBlock { Text = "Password:", FontSize = 12 });
        _txtPassword = new TextBox { Text = "123456", PasswordChar = '*', Watermark = "Enter password" };
        passPanel.Children.Add(_txtPassword);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 14, 0, 0)
        };

        var btnLogin = new Button
        {
            Content = "Login",
            Width = 110,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnLogin.Click += async (s, e) => await AttemptLoginAsync();

        var btnExit = new Button
        {
            Content = "Exit",
            Width = 110,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnExit.Click += (s, e) => Close();

        buttonPanel.Children.Add(btnLogin);
        buttonPanel.Children.Add(btnExit);

        mainPanel.Children.Add(lblHeader);
        mainPanel.Children.Add(lblSub);
        mainPanel.Children.Add(userPanel);
        mainPanel.Children.Add(passPanel);
        mainPanel.Children.Add(buttonPanel);

        Content = mainPanel;
    }

    private async System.Threading.Tasks.Task AttemptLoginAsync()
    {
        try
        {
            string username = _txtUsername.Text?.Trim() ?? string.Empty;
            string password = _txtPassword.Text ?? string.Empty;

            var emp = await _authService.LoginAsync(username, password);
            if (emp != null)
            {
                _onLoginSuccess(new EmployeeSessionState(emp.EmployeeID, emp.EmployeeName, emp.Role ?? "Staff"));
                Close();
            }
            else
            {
                await MessageBoxDialog.ShowAsync(this, "Invalid username or password. Please try again.", "Login Failed");
            }
        }
        catch (Exception ex)
        {
            await MessageBoxDialog.ShowAsync(this, ex.Message, "Authentication Error");
        }
    }
}
