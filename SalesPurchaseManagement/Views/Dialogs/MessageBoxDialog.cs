using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace SalesPurchaseManagement.Views.Dialogs;

/// <summary>
/// Simple desktop modal dialog replacement for MessageBox.Show in Avalonia.
/// </summary>
public static class MessageBoxDialog
{
    public static async Task ShowAsync(Window parent, string message, string title = "Information")
    {
        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 180,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var root = new StackPanel
        {
            Margin = new Thickness(24),
            Spacing = 16,
            VerticalAlignment = VerticalAlignment.Center
        };

        var tb = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13
        };

        var btnOk = new Button
        {
            Content = "OK",
            Width = 90,
            HorizontalAlignment = HorizontalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnOk.Click += (s, e) => dialog.Close();

        root.Children.Add(tb);
        root.Children.Add(btnOk);
        dialog.Content = root;

        await dialog.ShowDialog(parent);
    }

    public static async Task<bool> ShowConfirmAsync(Window parent, string message, string title = "Confirm")
    {
        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 180,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        bool result = false;

        var root = new StackPanel
        {
            Margin = new Thickness(24),
            Spacing = 18,
            VerticalAlignment = VerticalAlignment.Center
        };

        var tb = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13
        };

        var btnPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var btnYes = new Button
        {
            Content = "Yes",
            Width = 80,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnYes.Click += (s, e) => { result = true; dialog.Close(); };

        var btnNo = new Button
        {
            Content = "No",
            Width = 80,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnNo.Click += (s, e) => { result = false; dialog.Close(); };

        btnPanel.Children.Add(btnYes);
        btnPanel.Children.Add(btnNo);

        root.Children.Add(tb);
        root.Children.Add(btnPanel);
        dialog.Content = root;

        await dialog.ShowDialog(parent);
        return result;
    }
}
