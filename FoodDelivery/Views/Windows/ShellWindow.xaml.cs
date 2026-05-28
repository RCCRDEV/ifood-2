using FoodDelivery.Controllers;
using System.Windows;
using System.Windows.Controls;

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
        NavListBox.ItemsSource = _controller.GetNavItems();
        NavListBox.SelectedIndex = 0;
    }

    private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavListBox.SelectedItem is not ShellNavItem item) return;
        PageTitleTextBlock.Text = item.Label;
        _controller.Navigate(MainFrame, item.Route);
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        _controller.Logout(this);
    }
}
