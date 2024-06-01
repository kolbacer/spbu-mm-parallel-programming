using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Task5.GraphicChatUI.ViewModels;

namespace Task5.GraphicChatUI.Views;

public partial class ChatMessagesView : UserControl
{
    public ChatMessagesView()
    {
        InitializeComponent();
        MessageItemsControl.SizeChanged += MessageItemsControl_SizeChanged;
    }
    
    private void MessageItemsControl_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.HeightChanged)
        {
            MessageScrollViewer.ScrollToEnd();
        }
    } 
}