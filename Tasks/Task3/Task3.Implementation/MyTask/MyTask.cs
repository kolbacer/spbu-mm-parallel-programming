using Task3.Implementation.Common.Exceptions;
using Task3.Implementation.ThreadPool;

namespace Task3.Implementation.MyTask;

/// <summary>
/// Class containing a function to be executed in <see cref="T:Task3.Implementation.ThreadPool.MyThreadPool"/>.
/// Supports continuation.
/// </summary>
/// <typeparam name="TResult">Function return type</typeparam>
public class MyTask<TResult>: IMyTask
{
    private object Locker { get; } = new();
    
    /// <summary>
    /// Should only be accessed from the thread pool
    /// </summary>
    public MyThreadPool? ThreadPool { get; set; }
    private Func<TResult>? Func { get; set; }
    private event Action? OnCompleted;
    private event Action? OnFailed;
    
    private volatile bool isCompleted;
    
    /// <summary>
    /// Whether the task is completed
    /// </summary>
    public bool IsCompleted
    {
        get => isCompleted;
        private set => isCompleted = value;
    }
    
    private volatile bool isFailed;
    
    /// <summary>
    /// Whether the task is failed
    /// </summary>
    public bool IsFailed
    {
        get => isFailed;
        private set => isFailed = value;
    }
    
    private List<Exception> exceptions = new();

    private TResult? result;
    
    /// <summary>
    /// Get the result of the task. Blocks the calling thread until the task completes.
    /// </summary>
    /// <exception cref="AggregateException">Exceptions that caused a task to fail</exception>
    public TResult Result
    {
        get
        {
            while (!IsCompleted && !IsFailed) Thread.Yield();
            if (isFailed) throw new AggregateException(exceptions);
            return result!;
        }
        private set => result = value;
    }
    
    private MyTask()
    {
        IsCompleted = false;
        IsFailed = false;
    }

    /// <summary>
    /// Create a new task without queuing it in the thread pool
    /// </summary>
    /// <param name="func">Function to be executed</param>
    public MyTask(Func<TResult> func)
    {
        Func = func;
        IsCompleted = false;
        IsFailed = false;
    }

    /// <summary>
    /// Create a new task that takes as input the result of the current task.
    /// Will be executed in the same thread pool after the current task is completed.
    /// </summary>
    /// <param name="func">Continuation function</param>
    /// <typeparam name="TNewResult">Continuation function return type</typeparam>
    /// <returns>Continuation task</returns>
    public MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
    {
        MyTask<TNewResult> newTask = new MyTask<TNewResult>();
        
        Action doContinuation = () =>
        {
            newTask.Func = () => func(Result);
            ThreadPool!.Enqueue(newTask);
        };
        
        Action doFail = () =>
        {
            newTask.exceptions.AddRange(exceptions);
            newTask.exceptions.Add(new ParentTaskFailException(this));
            lock (newTask.Locker)
            {
                newTask.IsFailed = true;
            }
            newTask.OnFailed?.Invoke();
        };

        lock (Locker)
        {
            if (IsCompleted)
                doContinuation();
            else if (IsFailed)
                doFail();
            else
            {
                OnCompleted += doContinuation;
                OnFailed += doFail;
            }
        }
        
        return newTask; 
    }
    
    /// <summary>
    /// Start method for task. Should only be called from the thread pool.
    /// </summary>
    public void Start()
    {
        try
        {
            Result = Func();
            lock (Locker)
            {
                IsCompleted = true;
            }
            OnCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
            lock (Locker)
            {
                IsFailed = true;
            }
            OnFailed?.Invoke();
        }
    }

    /// <summary>
    /// Task cancellation method. Should only be called from the thread pool.
    /// </summary>
    public void Cancel()
    {
        lock (Locker)
        {
            if (IsCompleted || IsFailed) return;
            exceptions.Add(new TaskCancelledException());
            IsFailed = true;
            OnFailed?.Invoke();
        }
    }
}