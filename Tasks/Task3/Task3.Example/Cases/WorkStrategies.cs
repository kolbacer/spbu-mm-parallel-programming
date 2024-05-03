using Task3.Implementation.MyTask;
using Task3.Implementation.ThreadPool;

namespace Task3.Example.Cases;

public static class WorkStrategies
{
    public static void Run()
    {
        Console.WriteLine("//// Work strategies example ////");
        Console.WriteLine();
        
        int numOfThreads = 4;
        int numOfTasks = 20;
        
        List<Func<int>> funcs = Enumerable.Range(1, numOfTasks).Select<int, Func<int>>(i => () =>
        {
            Console.WriteLine($"Task{i} in thread {Environment.CurrentManagedThreadId}");
            Thread.Sleep(100);
            return 0;
        }).ToList();

        Console.WriteLine("Work sharing strategy:");
        using (MyThreadPool pool = new MyThreadPool(numOfThreads, WorkStrategy.WorkSharing))
        {
            foreach (var func in funcs)
            {
                pool.Enqueue(new MyTask<int>(func));
            }
            Thread.Sleep(2000);
        }

        Console.WriteLine();
        Console.WriteLine("Work stealing strategy:");
        using (MyThreadPool pool = new MyThreadPool(numOfThreads, WorkStrategy.WorkStealing))
        {
            foreach (var func in funcs)
            {
                pool.Enqueue(new MyTask<int>(func));
            }
            Thread.Sleep(2000);
        }

        Console.WriteLine();
    }
}