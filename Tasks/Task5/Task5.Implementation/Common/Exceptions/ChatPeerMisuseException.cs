namespace Task5.Implementation.Common.Exceptions;

public class ChatPeerMisuseException : Exception
{
    public ChatPeerMisuseException() : base() {}
    
    public ChatPeerMisuseException(string message) : base(message) {}
}