namespace Task3.Implementation.ThreadPool.WorkRunner;

/// <summary>
/// Encapsulates an algorithm for executing thread pool tasks according to some strategy.
/// </summary>
public interface IWorkRunner
{
    /// <summary>
    /// Start work
    /// </summary>
    void Run();
}