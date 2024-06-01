using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Task5.GraphicChatUI.Services;

namespace Task5.GraphicChatUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _messageInput = string.Empty;
    private bool _isMessageInputEnabled = false;
    private bool _isMessageSendEnabled = false;
    
    public ChatPeerService Service { get; }
    public ChatMessagesViewModel ChatMessages { get; }
    public UserPanelViewModel UserPanel { get; }
    
    public ReactiveCommand<Unit, Unit> EnterCommand { get; }
    
    public string MessageInput
    {
        get => _messageInput;
        set => this.RaiseAndSetIfChanged(ref _messageInput, value);
    }
    
    public bool IsMessageInputEnabled
    {
        get => _isMessageInputEnabled;
        set => this.RaiseAndSetIfChanged(ref _isMessageInputEnabled, value);
    }
    
    public bool IsMessageSendEnabled
    {
        get => _isMessageSendEnabled;
        set => this.RaiseAndSetIfChanged(ref _isMessageSendEnabled, value);
    }
    
    public MainWindowViewModel()
    {
        var service = new ChatPeerService();
        Service = service;
        ChatMessages = new ChatMessagesViewModel(service);
        UserPanel = new UserPanelViewModel(service);
        
        var isValidMessageInput = this.WhenAnyValue(
            x => x.MessageInput,
            x => !string.IsNullOrWhiteSpace(x));

        var isValidSend = isValidMessageInput.CombineLatest(
            Service.IsRunningObservable,
            (a, b) => a && b).DistinctUntilChanged();
        
        Service.IsRunningObservable.Subscribe(x => IsMessageInputEnabled = x);
        isValidSend.Subscribe(x => IsMessageSendEnabled = x);
        
        EnterCommand = ReactiveCommand.Create(OnEnterPressed);
    }
    
    public void SendMessage()
    {
        ChatMessages.SendMessage(MessageInput);
        MessageInput = string.Empty;
    }
    
    public void ClearChat()
    {
        ChatMessages.ClearMessages();
    }
    
    private void OnEnterPressed()
    {
        SendMessage();
    }
}