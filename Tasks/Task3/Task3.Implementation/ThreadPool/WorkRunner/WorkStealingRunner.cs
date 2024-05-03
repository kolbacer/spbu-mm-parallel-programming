using Task3.Implementation.MyTask;
using Task3.Implementation.Primitives.Deque;

namespace Task3.Implementation.ThreadPool.WorkRunner;

/// <summary>
/// Run thread pool tasks using the <b>work stealing</b> strategy.
/// Based on algorithm in Common/LocksContinued/WorkStealing/WorkStealingThread
/// </summary>
public class WorkStealingRunner: IWorkRunner
{
    private Dictionary<int, IThreadSafeDeque<IMyTask>> ThreadPoolQueues { get; }
    private Random random;
    private CancellationToken token;

    public WorkStealingRunner(Dictionary<int, IThreadSafeDeque<IMyTask>> queues, CancellationToken token)
    {
        ThreadPoolQueues = queues;
        random = new Random();
        this.token = token;
    }
    
    public void Run()
    {
        int me = Environment.CurrentManagedThreadId;
        Console.WriteLine($"### WorkerThread {me} started ###");
        
        while (!token.IsCancellationRequested)
        {
            while (ThreadPoolQueues[me].TryPopBottom(out var myTask))
            {
                myTask.Start();
                if (token.IsCancellationRequested) break;
            }

            IMyTask stolenTask;
            while (!ThreadPoolQueues[GetVictim()].TryPopTop(out stolenTask))
            {
                Thread.Yield();
                if (token.IsCancellationRequested) break;
            }
            stolenTask?.Start();
        }
        
        Cleanup();
        Console.WriteLine($"### WorkerThread {me} stopped ###");
    }

    private int GetVictim()
    {
        return ThreadPoolQueues.ElementAt(random.Next(ThreadPoolQueues.Keys.Count)).Key;
    }

    private void Cleanup()
    {
        IThreadSafeDeque<IMyTask> myQueue = ThreadPoolQueues[Environment.CurrentManagedThreadId];
        lock (myQueue.Locker)
        {
            while (myQueue.UnsafeTryPopBottom(out var task)) task.Cancel();   
        }
    }
}