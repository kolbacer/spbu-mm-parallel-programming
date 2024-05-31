namespace Task5.Implementation.Message;

/// <summary>
/// Class encapsulating a chat message.
/// </summary>
[Serializable]
public class ChatMessage
{
    public MessageType Type { get; }
    public string Sender { get; set; }
    public string Text { get; set; }
    
    public ChatMessage(MessageType type, string sender, string text)
    {
        Type = type;
        Sender = sender;
        Text = text;
    }
}