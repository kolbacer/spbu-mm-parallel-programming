using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Task5.GraphicChatUI.DataModel;
using Task5.GraphicChatUI.Services;
using Task5.Implementation.Message;

namespace Task5.GraphicChatUI.ViewModels;

public class ChatMessagesViewModel : ViewModelBase
{
    public ChatPeerService Service { get; }
    public ObservableCollection<LocalMessage> MessageList { get; }
    
    public ChatMessagesViewModel(ChatPeerService service)
    {
        MessageList = new ObservableCollection<LocalMessage>();
        Service = service;
        service.MessageHandler = ReceiveMessage;
        service.InfoHandler = ReceiveInfo;
    }

    public void ClearMessages()
    {
        MessageList.Clear();
    }

    public void SendMessage(string message)
    {
        MessageList.Add(new LocalMessage
        {
            Sender = Service.Username,
            Text = message,
            TimeStamp = DateTime.UtcNow,
            Type = LocalMessageType.You
        });
        Service.Send(message);
    }

    private void ReceiveMessage(ChatMessage chatMessage)
    {
        if (chatMessage.Type == MessageType.Text)
            MessageList.Add(new LocalMessage
            {
                Sender = chatMessage.Sender,
                Text = chatMessage.Text,
                TimeStamp = DateTime.UtcNow,
                Type = LocalMessageType.FromUser
            });
    }
    
    private void ReceiveInfo(string infoMessage)
    {
        MessageList.Add(new LocalMessage
        {
            Sender = "[info]",
            Text = infoMessage,
            TimeStamp = DateTime.UtcNow,
            Type = LocalMessageType.Info
        });
    }
    
}