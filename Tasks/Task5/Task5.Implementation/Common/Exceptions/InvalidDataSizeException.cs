namespace Task5.Implementation.Common.Exceptions;

public class InvalidDataSizeException : Exception
{
    public InvalidDataSizeException() : base() {}
    
    public InvalidDataSizeException(string message) : base(message) {}
}