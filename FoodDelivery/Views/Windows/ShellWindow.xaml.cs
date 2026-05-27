using FoodDelivery.Controllers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FoodDelivery.Views.Windows;

public partial class ShellWindow : Window
{
    private readonly ShellController _controller;

    public ShellWindow(ShellController controller)
    {
        _controller = controller;
        InitializeComponent();
        Loaded += ShellWindow_Loaded;
    }

    private void ShellWindow_Loaded(object sender, RoutedEventArgs e)
    {
        UserTitleTextBlock.Text = _controller.GetUserTitle();
        NavStack.Children.Clear();

        foreach (var item in _controller.GetNavItems())
        {
            var button = CreateNavButton(item);
            NavStack.Children.Add(button);
        }

        var first = _controller.GetNavItems().FirstOrDefault();
        if (first is not null)
            _controller.Navigate(MainFrame, first.Route);
    }

    private Button CreateNavButton(ShellNavItem item)
    {
        var icon = new TextBlock
        {
            Text = item.IconGlyph,
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            FontSize = 16,
            Width = 24,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = (Brush)FindResource("BrushText")
        };

        var text = new TextBlock
        {
            Text = item.Label,
            Margin = new Thickness(10, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = (Brush)FindResource("BrushText")
        };

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(2, 0, 0, 0)
        };
        panel.Children.Add(icon);
        panel.Children.Add(text);

        var button = new Button
        {
            Content = panel,
            Margin = new Thickness(0, 0, 0, 10),
            HorizontalContentAlignment = HorizontalAlignment.Left
        };

        button.Click += (_, _) => _controller.Navigate(MainFrame, item.Route);
        return button;
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        _controller.Logout(this);
    }
}

