namespace Task3.Implementation.Common.Exceptions;

public class InvalidNumberOfThreadsException : Exception
{
    public InvalidNumberOfThreadsException() : base() {}
    
    public InvalidNumberOfThreadsException(string message) : base(message) {}
}