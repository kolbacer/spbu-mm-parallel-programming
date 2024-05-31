using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Task5.Implementation.Common.Exceptions;
using Task5.Implementation.Message;
using Task5.Implementation.Utils;

namespace Task5.Implementation;

/// <summary>
/// Class representing a peer in the chat system.
/// TCP <see cref="Socket"/> instances are used for connection. 
/// </summary>
public class ChatPeer: IDisposable
{
    public string Username { get; private set; }
    public IPEndPoint IpEndPoint { get; private set; }
    public IPAddress IpAddress => IpEndPoint.Address;
    public int Port => IpEndPoint.Port;
    public int PeersConnected => _peers.Count;
    public bool IsRunning { get; private set; } = false;
    
    public Action<ChatMessage> MessageHandler { get; set; }
    public Action<string> InfoHandler { get; set; }

    private Socket _listenerSocket;
    private CancellationTokenSource? _listenerCancellation;
    
    private ConcurrentDictionary<EndPoint, Socket> _peers;
    private ConcurrentDictionary<EndPoint, CancellationTokenSource> _peerCancellations;
    private volatile bool _disposeCalled = false;
    private volatile bool _disposed = false;

    public ChatPeer(string username, IPEndPoint endPoint)
    {
        Username = username;
        IpEndPoint = endPoint;
        
        _peers = new ConcurrentDictionary<EndPoint, Socket>();
        _peerCancellations = new ConcurrentDictionary<EndPoint, CancellationTokenSource>();
        _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listenerSocket.Bind(endPoint);
    }

    public ChatPeer(string username, string ipAddress, int port) :
        this(username, new IPEndPoint(IPAddress.Parse(ipAddress), port))
    {
    }

    /// <summary>
    /// Start listening for incoming connections to the peer.
    /// Must be launched before any other actions for the chat to work correctly.
    /// </summary>
    /// <exception cref="ChatPeerMisuseException">Throws if ChatPeer is already running.</exception>
    public void Start()
    {
        if (IsRunning)
            throw new ChatPeerMisuseException("ChatPeer is already running");
        
        _listenerCancellation = new CancellationTokenSource();
        _listenerSocket.Listen();
        IsRunning = true;
        AcceptPeersAsync();
    }

    private async void AcceptPeersAsync()
    {
        try
        {
            while (!_listenerCancellation!.IsCancellationRequested)
            {
                Socket clientSocket = await _listenerSocket.AcceptAsync(_listenerCancellation.Token);
                CancellationTokenSource cts = new CancellationTokenSource();
                HandlePeerAsync(clientSocket, cts);
            }
        }
        catch (OperationCanceledException)
        {
            InfoHandler?.Invoke("Listening cancelled");
        }
        finally
        {
            _listenerCancellation!.Dispose();
            if (_disposeCalled) StopListener();
        }
    }

    /// <summary>
    /// Handle incoming messages for the specified peer.
    /// Runs concurrently for each peer.
    /// </summary>
    /// <param name="peerSocket">Socket for connected peer</param>
    /// <param name="cts">Cancellation token sourse</param>
    /// <exception cref="InvalidMessageTypeException">Unexpected message type was encountered</exception>
    private async void HandlePeerAsync(Socket peerSocket, CancellationTokenSource cts)
    {
        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                ChatMessage chatMessage = await ReceiveMessageAsync(peerSocket);

                switch (chatMessage.Type)
                {
                    case MessageType.ConnectAllPeers:
                        foreach (var (endPoint, peer) in _peers)
                        {
                            if (endPoint.Equals(IPEndPoint.Parse(chatMessage.Text))) continue;
                            await SendMessage(new ChatMessage(MessageType.ConnectOnePeer, Username, chatMessage.Text), peer);
                        }
                        break;
                    case MessageType.ConnectOnePeer:
                        await ConnectToPeer(IPEndPoint.Parse(chatMessage.Text));
                        break;
                    case MessageType.AddPeer:
                        InfoHandler?.Invoke($"Add peer: {IPEndPoint.Parse(chatMessage.Text)}");
                        _peers.TryAdd(IPEndPoint.Parse(chatMessage.Text), peerSocket);
                        _peerCancellations.TryAdd(IPEndPoint.Parse(chatMessage.Text), cts);
                        break;
                    case MessageType.RemovePeer:
                        StopPeerHandling(IPEndPoint.Parse(chatMessage.Text));
                        break;
                    case MessageType.Text:
                        break;
                    default:
                        throw new InvalidMessageTypeException();
                }

                MessageHandler?.Invoke(chatMessage);
            }
        }
        catch (Exception ex)
        {
            if (!(cts.Token.IsCancellationRequested || ex is SocketException)) throw;
            // Client closed connection
        }
        finally
        {
            EndPoint endPoint = _peers.FirstOrDefault(x => x.Value == peerSocket).Key;
            InfoHandler?.Invoke($"Peer {endPoint} disconnected");
            RemovePeer(endPoint);
        }
    }

    /// <summary>
    /// Connect to other peer.
    /// </summary>
    /// <param name="ipAddress">Peer IP address</param>
    /// <param name="port">Peer port</param>
    /// <param name="requestChatConnection">If true, asks the peer to send
    /// a request to all peers connected to it to connect to your peer.</param>
    public async Task ConnectToPeer(string ipAddress, int port, bool requestChatConnection = false)
    {
        await ConnectToPeer(new IPEndPoint(IPAddress.Parse(ipAddress), port), requestChatConnection);
    }
    
    /// <summary>
    /// Connect to other peer.
    /// </summary>
    /// <param name="endPoint">Peer IP endpoint</param>
    /// <param name="requestChatConnection">If true, asks the peer to send
    /// a request to all peers connected to it to connect to your peer.</param>
    /// <exception cref="ChatPeerMisuseException">Throws if peer is already connected.</exception>
    public async Task ConnectToPeer(IPEndPoint endPoint, bool requestChatConnection = false)
    {
        if (_peers.ContainsKey(endPoint))
            throw new ChatPeerMisuseException($"Peer {endPoint} is already connected");
        Socket peerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        peerSocket.Connect(endPoint);
        InfoHandler?.Invoke($"Connect to peer: {endPoint}");
        _peers.TryAdd(endPoint, peerSocket);
        CancellationTokenSource cts = new CancellationTokenSource();
        _peerCancellations.TryAdd(endPoint, cts);
        if (requestChatConnection)
        {
            await SendMessage(new ChatMessage(MessageType.ConnectAllPeers, Username, $"{IpAddress}:{Port}"), peerSocket);
            foreach (var (peerEndPoint, peer) in _peers)
            {
                if (endPoint.Equals(peerEndPoint)) continue;
                await SendMessage(new ChatMessage(MessageType.ConnectOnePeer, Username, endPoint.ToString()), peer);
            }
        }
        HandlePeerAsync(peerSocket, cts);
        await SendMessage(new ChatMessage(MessageType.AddPeer, Username, $"{IpAddress}:{Port}"), peerSocket);
    }
    
    /// <summary>
    /// Stop handle peer messages and notify it about it.
    /// </summary>
    /// <param name="endPoint">Peer endpoint</param>
    /// <exception cref="NoSuchPeerException">Throws if peer is not connected.</exception>
    public async Task DisconnectFromPeer(EndPoint endPoint)
    {
        if (!_peers.TryGetValue(endPoint, out var peerSocket))
            throw new NoSuchPeerException($"Peer {endPoint} is not connected");
        
        await SendMessage(new ChatMessage(MessageType.RemovePeer, Username, $"{IpAddress}:{Port}"), peerSocket);
        StopPeerHandling(endPoint);
    }

    private void StopPeerHandling(EndPoint endPoint)
    {
        if (!_peers.TryGetValue(endPoint, out var peerSocket))
            throw new NoSuchPeerException($"Peer {endPoint} is not connected");
        
        _peerCancellations[endPoint].Cancel();
    }
    
    /// <summary>
    /// Close peer socket and remove it.
    /// </summary>
    /// <param name="endPoint">Peer endpoint</param>
    private void RemovePeer(EndPoint endPoint)
    {
        _peerCancellations[endPoint].Dispose();
        _peerCancellations.Remove(endPoint, out _);
        try
        {
            if (_peers[endPoint].Connected) 
                _peers[endPoint].Shutdown(SocketShutdown.Both);
        }
        finally
        {
            _peers[endPoint].Close();
        }
        
        _peers.Remove(endPoint, out _);
    }
    
    /// <summary>
    /// Receive a message using custom protocol.
    /// First 4 bytes determines data size and next bytes of this size is an actual message.
    /// </summary>
    /// <param name="socket">Socket from which to receive a message</param>
    /// <returns>Task, returning recieved <see cref="ChatMessage"/></returns>
    /// <exception cref="InvalidDataSizeException">Unexpected data size was encountered</exception>
    private async Task<ChatMessage> ReceiveMessageAsync(Socket socket)
    {
        byte[] dataSizeBytes = new byte[4];
        int bytesRead = await socket.ReceiveAsync(dataSizeBytes, SocketFlags.None);
        if (bytesRead != 4) 
            throw new InvalidDataSizeException($"Got {bytesRead} bytes instead of 4");

        int dataSize = BitConverter.ToInt32(dataSizeBytes, 0);
        byte[] data = new byte[dataSize];
        bytesRead = 0;

        while (bytesRead < dataSize)
        {
            int read = await socket.ReceiveAsync(new ArraySegment<byte>(data, bytesRead, dataSize - bytesRead), SocketFlags.None);
            if (read == 0) throw new Exception("Connection closed unexpectedly");
            bytesRead += read;
        }

        return MessageSerializer.Deserialize(data)!;
    }
    
    /// <summary>
    /// Send a message using custom protocol.
    /// First 4 bytes determines data size and next bytes of this size is an actual message.
    /// </summary>
    /// <param name="chatMessage">Message to send</param>
    /// <param name="peer">Peer socket to send to</param>
    public async Task SendMessage(ChatMessage chatMessage, Socket peer)
    {
        byte[] data = MessageSerializer.Serialize(chatMessage);
        byte[] dataSizeBytes = BitConverter.GetBytes(data.Length);
        
        await peer.SendAsync(dataSizeBytes, SocketFlags.None);
        await peer.SendAsync(data, SocketFlags.None);
    }

    /// <summary>
    /// Send a message to all peers using custom protocol.
    /// First 4 bytes determines data size and next bytes of this size is an actual message.
    /// </summary>
    /// <param name="chatMessage">Message to send</param>
    public async Task SendMessageToAll(ChatMessage chatMessage)
    {
        byte[] data = MessageSerializer.Serialize(chatMessage);
        byte[] dataSizeBytes = BitConverter.GetBytes(data.Length);
        
        foreach (var (endPoint, peerSocket) in _peers)
        {
            await peerSocket.SendAsync(dataSizeBytes, SocketFlags.None);
            await peerSocket.SendAsync(data, SocketFlags.None);
        }
    }
    
    /// <summary>
    /// Send a text message using custom protocol.
    /// First 4 bytes determines data size and next bytes of this size is an actual message.
    /// </summary>
    /// <param name="textMessage">Text to send</param>
    /// <param name="peer">Peer socket to send to</param>
    public async Task SendTextMessage(string textMessage, Socket peer)
    {
        await SendMessage(new ChatMessage(MessageType.Text, Username, textMessage), peer);
    }
    
    /// <summary>
    /// Send a text message to all peers using custom protocol.
    /// First 4 bytes determines data size and next bytes of this size is an actual message.
    /// </summary>
    /// <param name="textMessage">Text to send</param>
    public async Task SendTextMessageToAll(string textMessage)
    {
        await SendMessageToAll(new ChatMessage(MessageType.Text, Username, textMessage));
    }

    /// <summary>
    /// Stop chat peer listening incoming messages and disconnect from all other peers.
    /// </summary>
    /// <exception cref="ChatPeerMisuseException">Throws if ChatPeer is already stopped.</exception>
    public async void Stop()
    {
        if (!IsRunning)
            throw new ChatPeerMisuseException("ChatPeer is already stopped");

        IsRunning = false;
        
        foreach (var (endPoint, peerSocket) in _peers)
        {
            await DisconnectFromPeer(endPoint);
        }
        _listenerCancellation!.Cancel();
    }

    private void StopListener()
    {
        try
        {
            if (_listenerSocket.Connected) 
                _listenerSocket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            _listenerSocket.Close();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        _disposeCalled = true;
        Stop();
        while (!_disposed)
            Thread.Yield();
    }
}