using System.Net;
using Task5.Implementation;

namespace Task5.UnitTests;

/// <summary>
/// Unit tests for <see cref="ChatPeer"/>.
/// Tests involve asynchronous socket communication, so a <see cref="Timeout"/> should be set.
/// </summary>
public class ChatPeerTest
{
    private static string IP { get; } = "127.0.0.1";
    private static int StartPort { get; } = 5000;
    private static int PeerCount { get; } = 3;
    private static List<ChatPeer> Peers { get; } = new(PeerCount);
    private static int Timeout { get; } = 300;
    
    [SetUp]
    public void Setup()
    {
        for (int i = 0; i < PeerCount; ++i)
        {
            Peers.Add(new ChatPeer($"Peer{i}", IP, StartPort + i));
            Peers[i].Start();
        }
    }
    
    [TearDown]
    public void Teardown()
    {
        foreach (var chatPeer in Peers)
        {
            chatPeer.Dispose();
        }
        Peers.Clear();
    }
    
    [Test]
    public async Task TwoChatPeersConnectionTest()
    {
        await Peers[0].ConnectToPeer(IPEndPoint.Parse($"{IP}:{StartPort+1}"), true);
        Thread.Sleep(Timeout);
        Assert.That(Peers[0].PeersConnected, Is.EqualTo(1));
        Assert.That(Peers[1].PeersConnected, Is.EqualTo(1));
        
        await Peers[0].DisconnectFromPeer(IPEndPoint.Parse($"{IP}:{StartPort+1}"));
        Thread.Sleep(Timeout);
        Assert.That(Peers[0].PeersConnected, Is.EqualTo(0));
        Assert.That(Peers[1].PeersConnected, Is.EqualTo(0));
    }
    
    [Test]
    public async Task ThreeChatPeersConnectionTest()
    {
        await Peers[0].ConnectToPeer(IPEndPoint.Parse($"{IP}:{StartPort+1}"), true);
        await Peers[1].ConnectToPeer(IPEndPoint.Parse($"{IP}:{StartPort+2}"), true);
        Thread.Sleep(Timeout);
        Assert.That(Peers[0].PeersConnected, Is.EqualTo(2));
        Assert.That(Peers[1].PeersConnected, Is.EqualTo(2));
        Assert.That(Peers[2].PeersConnected, Is.EqualTo(2));
        
        await Peers[0].DisconnectFromPeer(IPEndPoint.Parse($"{IP}:{StartPort+1}"));
        await Peers[0].DisconnectFromPeer(IPEndPoint.Parse($"{IP}:{StartPort+2}"));
        Thread.Sleep(Timeout);
        Assert.That(Peers[0].PeersConnected, Is.EqualTo(0));
        Assert.That(Peers[1].PeersConnected, Is.EqualTo(1));
        Assert.That(Peers[2].PeersConnected, Is.EqualTo(1));
    }
    
    [Test]
    public async Task MessageSendingTest()
    {
        await Peers[0].ConnectToPeer(IPEndPoint.Parse($"{IP}:{StartPort+1}"), true);
        await Peers[1].ConnectToPeer(IPEndPoint.Parse($"{IP}:{StartPort+2}"), true);
        Thread.Sleep(Timeout);

        string sendedMessage = "Test message";
        string? receivedMessage1 = null;
        string? receivedMessage2 = null;
        Peers[1].MessageHandler = (message => receivedMessage1 = message.Text);
        Peers[2].MessageHandler = (message => receivedMessage2 = message.Text);
        await Peers[0].SendTextMessageToAll(sendedMessage);
        Thread.Sleep(Timeout);
        
        Assert.That(sendedMessage, Is.EqualTo(receivedMessage1));
        Assert.That(sendedMessage, Is.EqualTo(receivedMessage2));
    }
}