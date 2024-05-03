using System.Collections;
using System.Runtime.CompilerServices;
using Task3.Implementation.Common.Exceptions;
using Task3.Implementation.MyTask;
using Task3.Implementation.Primitives;
using Task3.Implementation.Primitives.Deque;
using Task3.Implementation.Primitives.Queue;
using Task3.Implementation.ThreadPool.WorkRunner;

[assembly:InternalsVisibleTo("Task3.UnitTests")]
namespace Task3.Implementation.ThreadPool;

/// <summary>
/// Thread pool for <see cref="T:Task3.Implementation.MyTask.MyTask`1"/> execution.
/// Supports different scheduling strategies.
/// </summary>
public class MyThreadPool : IDisposable
{
    internal Dictionary<int, Thread> Threads { get; }
    private CancellationTokenSource CTS { get; }
    private IDictionary ThreadQueues { get; } 
    private IWorkRunner Runner { get; }
    private object Locker { get; } = new();
    private object DisposeLocker { get; } = new();
    private volatile bool disposeCalled = false;
    private volatile bool disposed = false;
    
    /// <summary>
    /// Number of worker threads in thread pool
    /// </summary>
    public int NumOfThreads { get; }
    
    /// <summary>
    /// Create a new thread pool with the specified number of worker threads and scheduling strategy.
    /// </summary>
    /// <param name="numOfThreads">Number of worker threads</param>
    /// <param name="strategy">Scheduling strategy</param>
    /// <exception cref="InvalidNumberOfThreadsException">Number of worker threads is incorrect</exception>
    /// <exception cref="InvalidWorkStrategyException">Scheduling strategy is not supported</exception>
    public MyThreadPool(int numOfThreads, WorkStrategy strategy)
    {
        if (numOfThreads <= 0)
            throw new InvalidNumberOfThreadsException("Number of threads must be > 0");
        
        NumOfThreads = numOfThreads;
        CTS = new CancellationTokenSource();
        Threads = new Dictionary<int, Thread>(numOfThreads);
        
        switch (strategy)
        {
            case WorkStrategy.WorkSharing:
                ThreadQueues = new Dictionary<int, IThreadSafeQueue<IMyTask>>(NumOfThreads);
                Runner = new WorkSharingRunner((Dictionary<int, IThreadSafeQueue<IMyTask>>)ThreadQueues, CTS.Token);
                break;
            case WorkStrategy.WorkStealing:
                ThreadQueues = new Dictionary<int, IThreadSafeDeque<IMyTask>>(NumOfThreads);
                Runner = new WorkStealingRunner((Dictionary<int, IThreadSafeDeque<IMyTask>>)ThreadQueues, CTS.Token);
                break;
            default:
                throw new InvalidWorkStrategyException();
        }
        
        for (int i = 0; i < NumOfThreads; ++i)
        {
            Thread thread = new Thread(Runner.Run);
            Threads.Add(thread.ManagedThreadId, thread);
            ThreadQueues.Add(thread.ManagedThreadId, new ThreadSafeDeque<IMyTask>());
        }
        
        Console.WriteLine($"### ThreadPool started in thread {Environment.CurrentManagedThreadId} ###");
        foreach (Thread thread in Threads.Values)
            thread.Start();
    }

    /// <summary>
    /// Add a task to the execution queue.
    /// Enqueuing is performed to the least loaded worker thread.
    /// </summary>
    /// <param name="task">Task to be executed</param>
    /// <typeparam name="TResult">Task result type</typeparam>
    public void Enqueue<TResult>(MyTask<TResult> task)
    {
        bool disposeLockTaken = false;
        try
        {
            Monitor.TryEnter(DisposeLocker, ref disposeLockTaken);
            if (!disposeLockTaken && disposeCalled) return;
            if (disposed) return;
            
            lock (Locker)
            {
                task.ThreadPool = this;
                ThreadSafeDeque<IMyTask>? minQueue = null;

                foreach (ThreadSafeDeque<IMyTask> queue in ThreadQueues.Values)
                {
                    if (minQueue == null || queue.Count < minQueue.Count)
                        minQueue = queue;
                }

                minQueue!.Enqueue(task);   
            }
        }
        finally
        {
            if (disposeLockTaken) Monitor.Exit(DisposeLocker);
        }
    }

    /// <summary>
    /// Create a new task and immediately enqueue it
    /// </summary>
    /// <param name="func">Task function</param>
    /// <typeparam name="TResult">Task return type</typeparam>
    /// <returns>New created and enqueued task</returns>
    public MyTask<TResult> StartNewTask<TResult>(Func<TResult> func)
    {
        MyTask<TResult> task = new MyTask<TResult>(func);
        Enqueue(task);
        return task;
    }

    /// <summary>
    /// Cancel the pool tasks and wait for the worker threads to stop.
    /// Currently executing tasks run until completion, and other tasks in the queue get an exception.
    /// New tasks are not enqueued.
    /// </summary>
    public void Dispose()
    {
        disposeCalled = true;
        lock (DisposeLocker)
        {
            if (disposed) return;
            CTS.Cancel();
            foreach (Thread thread in Threads.Values)
                thread.Join();
            Console.WriteLine($"### ThreadPool stopped in thread {Environment.CurrentManagedThreadId} ###");
            disposed = true;   
        }
    }
}