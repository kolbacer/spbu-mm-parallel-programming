using System.Text.Json;
using Task5.Implementation.Message;

namespace Task5.Implementation.Utils;

/// <summary>
/// Helper class for message serialization.
/// </summary>
public static class MessageSerializer
{
    public static byte[] Serialize(ChatMessage chatMessage)
    {
        return JsonSerializer.SerializeToUtf8Bytes(chatMessage);
    }
    
    public static ChatMessage? Deserialize(byte[] data)
    {
        return JsonSerializer.Deserialize<ChatMessage>(data);
    }
}