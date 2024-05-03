namespace Task3.Implementation.MyTask;

/// <summary>
/// Interface for generic class <see cref="T:Task3.Implementation.MyTask.MyTask`1"/>
/// </summary>
public interface IMyTask
{
    bool IsCompleted { get; }
    void Start();
    void Cancel();
}