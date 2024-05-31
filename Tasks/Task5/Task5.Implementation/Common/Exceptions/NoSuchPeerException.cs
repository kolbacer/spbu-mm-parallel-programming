namespace Task5.Implementation.Common.Exceptions;

public class NoSuchPeerException : Exception
{
    public NoSuchPeerException() : base() {}
    
    public NoSuchPeerException(string message) : base(message) {}
}