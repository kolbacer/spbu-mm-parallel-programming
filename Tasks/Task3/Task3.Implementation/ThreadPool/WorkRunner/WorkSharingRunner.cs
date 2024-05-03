using Task3.Implementation.MyTask;
using Task3.Implementation.Primitives.Queue;

namespace Task3.Implementation.ThreadPool.WorkRunner;

/// <summary>
/// Run thread pool tasks using the <b>work sharing</b> strategy.
/// Based on algorithm in Common/LocksContinued/WorkStealing/WorkSharingThread.
/// </summary>
public class WorkSharingRunner: IWorkRunner
{
    private Dictionary<int, IThreadSafeQueue<IMyTask>> ThreadPoolQueues { get; }
    private Random random;
    private const int THRESHOLD = 42;
    private CancellationToken token;
    
    public WorkSharingRunner(Dictionary<int, IThreadSafeQueue<IMyTask>> queues, CancellationToken token)
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
            if (ThreadPoolQueues[me].TryDequeue(out var task)) task.Start();
            int size = ThreadPoolQueues[me].Count;
            if (random.Next(size + 1) == size)
            {
                int victim = GetVictim();
                int min = (victim <= me) ? victim : me;
                int max = (victim <= me) ? me : victim;
                lock (ThreadPoolQueues[min].Locker)
                {
                    lock (ThreadPoolQueues[max].Locker)
                    {
                        Balance(ThreadPoolQueues[min], ThreadPoolQueues[max]);
                    }
                }
            }
        }
        
        Cleanup();
        Console.WriteLine($"### WorkerThread {me} stopped ###");
    }
    
    private int GetVictim()
    {
        return ThreadPoolQueues.ElementAt(random.Next(ThreadPoolQueues.Keys.Count)).Key;
    }
    
    private void Balance(IThreadSafeQueue<IMyTask> q0, IThreadSafeQueue<IMyTask> q1)
    {
        var qMin = (q0.Count < q1.Count) ? q0 : q1;
        var qMax = (q0.Count < q1.Count) ? q1 : q0;
        int diff = qMax.Count - qMin.Count;
        if (diff > THRESHOLD)
            while (qMax.Count > qMin.Count)
                qMin.UnsafeEnqueue(qMax.UnsafeDequeue());
    }

    private void Cleanup()
    {
        IThreadSafeQueue<IMyTask> myQueue = ThreadPoolQueues[Environment.CurrentManagedThreadId];
        lock (myQueue.Locker)
        {
            while (myQueue.UnsafeTryDequeue(out var task)) task.Cancel();
        }
    }
}