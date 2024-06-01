using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Task5.GraphicChatUI.ViewModels;

namespace Task5.GraphicChatUI.Views;

public partial class AvailableNetworksWindow : Window
{
    public AvailableNetworksWindow()
    {
        InitializeComponent();
        DataContext = new AvailableNetworksViewModel();
    }

    public AvailableNetworksWindow(AvailableNetworksViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}