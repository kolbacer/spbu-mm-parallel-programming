namespace Task4.Implementation.Common.Exceptions;

public class InvalidHashCodeException : Exception
{
    public InvalidHashCodeException() : base() {}
    
    public InvalidHashCodeException(string message) : base(message) {}
}