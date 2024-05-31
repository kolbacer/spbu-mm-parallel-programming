using Task5.Implementation.Message;
using Task5.Implementation.Utils;

namespace Task5.UnitTests;

/// <summary>
/// Unit tests for <see cref="ChatMessage"/> and related utils.
/// </summary>
public class ChatMessageTest
{

    [Test]
    public void ChatMessageSerializationTest()
    {
        const MessageType type = MessageType.Text;
        const string sender = "TestSender";
        const string text = "test text";

        ChatMessage chatMessage = new ChatMessage(type, sender, text);
        Assert.That(chatMessage.Type, Is.EqualTo(type));
        Assert.That(chatMessage.Sender, Is.EqualTo(sender));
        Assert.That(chatMessage.Text, Is.EqualTo(text));

        byte[] serializedMessage = MessageSerializer.Serialize(chatMessage);
        ChatMessage deserializedMessage = MessageSerializer.Deserialize(serializedMessage)!;
        Assert.That(deserializedMessage.Type, Is.EqualTo(type));
        Assert.That(deserializedMessage.Sender, Is.EqualTo(sender));
        Assert.That(deserializedMessage.Text, Is.EqualTo(text));
    }
}