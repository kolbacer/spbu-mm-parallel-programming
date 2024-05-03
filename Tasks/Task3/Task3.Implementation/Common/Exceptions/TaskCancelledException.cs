namespace Task3.Implementation.Common.Exceptions;

public class TaskCancelledException : Exception
{
    public TaskCancelledException() : base() {}
    
    public TaskCancelledException(string message) : base(message) {}
}