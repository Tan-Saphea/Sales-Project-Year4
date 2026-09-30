using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using SalesPurchaseManagement.Views;

namespace SalesPurchaseManagement;

public class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());

        try
        {
            Styles.Add(new StyleInclude(new Uri("avares://SalesPurchaseManagement/App.cs"))
            {
                Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml")
            });
        }
        catch
        {
            // Fallback if resource dictionary uri is not present
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Start by displaying the Login window
            var loginForm = new LoginForm(session =>
            {
                var mainForm = new MainForm();
                desktop.MainWindow = mainForm;
                mainForm.Show();
            });

            desktop.MainWindow = loginForm;
            loginForm.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
