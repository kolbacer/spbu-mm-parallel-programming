using System.Net;
using Task5.Implementation;
using Task5.Implementation.Message;

namespace Task5.ConsoleChatUI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("####### Welcome! Enter your info below #######");
        Console.Write("Username: ");
        string username = Console.ReadLine()!.Trim();
        ChatPeer chatPeer;
        while (true)
        {
            try
            {
                Console.Write("Endpoint (<IP>:<Port>): ");
                IPEndPoint endPoint = IPEndPoint.Parse(Console.ReadLine()!.Trim());
                chatPeer = new ChatPeer(username, endPoint);
                break;
            }
            catch (Exception ex)
            {
                ErrorHandler($"Can't create ChatPeer. Cause: {ex.Message}");
                ErrorHandler($"Try again.");
            }
        }
        
        chatPeer.MessageHandler = MessageHandler;
        chatPeer.InfoHandler = InfoHandler;
        chatPeer.Start();
        
        String helpString = """
                            ####### Available commands: #######
                            /connect <IP>:<port> - enter the chat by peer
                            /exit - exit chat and disconnect all peers
                            Type any other string to send message
                            """;
        Console.WriteLine(helpString);

        while (true)
        {
            string query = Console.ReadLine()!.Trim();
            if (query.StartsWith("/connect "))
            {
                string ipAndPort = query.Remove(0, 8).Trim();
                string peerIp;
                int peerPort;
                try
                {
                    string[] tokens = ipAndPort.Split(':');
                    peerIp = tokens[0];
                    peerPort = Int32.Parse(tokens[1]);
                }
                catch (Exception)
                {
                    ErrorHandler("invalid '/connect' usage");
                    continue;
                }

                try
                {
                    await chatPeer.ConnectToPeer(peerIp, peerPort, true);
                }
                catch (Exception ex)
                {
                    ErrorHandler($"Can't connect to peer. Cause: {ex.Message}");
                }
            }
            else if (query.StartsWith("/exit"))
            {
                break;
            }
            else
            {
                await chatPeer.SendTextMessageToAll(query);
            }
        }
        
        chatPeer.Dispose();
    }

    public static void MessageHandler(ChatMessage chatMessage)
    {
        if (chatMessage.Type == MessageType.Text)
        {
            Console.WriteLine($"{chatMessage.Sender}: {chatMessage.Text}");
        }
    }
    
    public static void InfoHandler(string info)
    {
        Console.WriteLine($"[info]: {info}");
    }

    public static void ErrorHandler(string error)
    {
        Console.WriteLine($"[error]: {error}"); 
    }
}