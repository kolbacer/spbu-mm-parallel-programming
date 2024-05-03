using Task3.Implementation.MyTask;

namespace Task3.Implementation.Common.Exceptions;

public class ParentTaskFailException : Exception
{
    public IMyTask ParentTask { get; }
    
    public ParentTaskFailException() : base() {}
    
    public ParentTaskFailException(string message) : base(message) {}

    public ParentTaskFailException(IMyTask parentTask) : base()
    {
        ParentTask = parentTask;
    }
    
    public ParentTaskFailException(string message, IMyTask parentTask) : base(message)
    {
        ParentTask = parentTask;
    }
}