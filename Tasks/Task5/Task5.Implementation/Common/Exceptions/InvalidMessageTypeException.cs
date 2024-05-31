namespace Task5.Implementation.Common.Exceptions;

public class InvalidMessageTypeException : Exception
{
    public InvalidMessageTypeException() : base() {}
    
    public InvalidMessageTypeException(string message) : base(message) {}
}