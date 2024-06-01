using System;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Task5.Implementation;
using Task5.Implementation.Message;

namespace Task5.GraphicChatUI.Services;

public class ChatPeerService
{
    public string Username => IsRunning ? Peer!.Username : String.Empty;
    public IPEndPoint? IpEndPoint => IsRunning ? Peer!.IpEndPoint : null;
    
    public Action<ChatMessage> MessageHandler { get; set; }
    public Action<string> InfoHandler { get; set; }
    
    private readonly BehaviorSubject<bool> _isRunningSubject = new(false);
    public bool IsRunning
    {
        get => _isRunningSubject.Value;
        private set => _isRunningSubject.OnNext(value);
    }
    public IObservable<bool> IsRunningObservable => _isRunningSubject;

    private ChatPeer? Peer { get; set; }

    public void Start(string username, IPEndPoint endPoint)
    {
        Peer = new ChatPeer(username, endPoint);
        Peer.MessageHandler = MessageHandler;
        Peer.InfoHandler = InfoHandler;
        Task.Run(Peer.Start);
        IsRunning = true;
    }
    
    public void Stop()
    {
        IsRunning = false;
        Peer?.Dispose();
        Peer = null;
    }

    public void Connect(string peerIp, int peerPort)
    {
        Peer?.ConnectToPeer(peerIp, peerPort, true);
    }

    public void Send(string message)
    {
        Peer?.SendTextMessageToAll(message);
    }
    
}