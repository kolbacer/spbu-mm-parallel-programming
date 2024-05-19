namespace Task4.Implementation.Common.Exceptions;

public class InvalidCapacityException : Exception
{
    public InvalidCapacityException() : base() {}
    
    public InvalidCapacityException(string message) : base(message) {}
}