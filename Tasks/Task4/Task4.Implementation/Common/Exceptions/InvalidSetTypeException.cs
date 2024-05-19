namespace Task4.Implementation.Common.Exceptions;

public class InvalidSetTypeException : Exception
{
    public InvalidSetTypeException() : base() {}
    
    public InvalidSetTypeException(string message) : base(message) {}
}