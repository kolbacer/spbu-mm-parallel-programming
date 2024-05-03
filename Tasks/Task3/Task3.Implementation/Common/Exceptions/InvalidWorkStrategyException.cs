namespace Task3.Implementation.Common.Exceptions;

public class InvalidWorkStrategyException : Exception
{
    public InvalidWorkStrategyException() : base() {}
    
    public InvalidWorkStrategyException(string message) : base(message) {}
}