using System.Windows;
using System.Windows.Controls;

namespace WorldCup.WPF;

internal sealed class ConfirmDialog : Window
{
    public ConfirmDialog(string message, string title, string confirmText, string cancelText)
    {
        Title                 = title;
        Width                 = 380;
        SizeToContent         = SizeToContent.Height;
        MinHeight             = 130;
        ResizeMode            = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        WindowStyle           = WindowStyle.ToolWindow;

        var root = new StackPanel { Margin = new Thickness(20, 20, 20, 12) };

        root.Children.Add(new TextBlock
        {
            Text        = message,
            TextWrapping = TextWrapping.Wrap,
            Margin      = new Thickness(0, 0, 0, 16)
        });

        var row = new StackPanel
        {
            Orientation         = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var btnOk = new Button
        {
            Content   = confirmText,
            MinWidth  = 80,
            Padding   = new Thickness(8, 4, 8, 4),
            Margin    = new Thickness(0, 0, 8, 0),
            IsDefault = true
        };
        btnOk.Click += (_, _) => DialogResult = true;

        var btnCancel = new Button
        {
            Content  = cancelText,
            MinWidth = 80,
            Padding  = new Thickness(8, 4, 8, 4),
            IsCancel = true
        };

        row.Children.Add(btnOk);
        row.Children.Add(btnCancel);
        root.Children.Add(row);
        Content = root;
    }
}
