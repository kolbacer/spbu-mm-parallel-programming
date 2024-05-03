using Task3.Implementation.MyTask;
using Task3.Implementation.ThreadPool;

namespace Task3.Example.Cases;

public static class Continuation
{
    public static void Run()
    {
        Console.WriteLine("//// Continuation example ////");
        Console.WriteLine();

        int numOfThreads = 8;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);

        Console.WriteLine("Long run:");
        int initValue = 5;
        MyTask<int> longTask = pool.StartNewTask(() =>
        {
            Console.Write("Step 1... ");
            Thread.Sleep(500);
            Console.WriteLine($"result: {initValue}");
            return initValue;
        }).ContinueWith(x =>
        {
            Console.Write("Step 2... ");
            Thread.Sleep(500);
            Console.WriteLine($"result: {x * 2}");
            return x * 2;
        }).ContinueWith(x =>
        {
            Console.Write("Step 3... ");
            Thread.Sleep(500);
            Console.WriteLine($"result: {x + 7}");
            return x + 7;
        });

        Console.WriteLine("Some text after creating tasks to demonstrate non-blocking continuation...");
        Console.WriteLine($"Final result: {longTask.Result}");
        Console.WriteLine();

        Console.WriteLine("Multiple continuations of the same task:");
        MyTask<int> originTask = pool.StartNewTask(() =>
        {
            Thread.Sleep(500);
            return 10;
        });

        MyTask<int> task1 = originTask.ContinueWith(x => x * 2);
        Thread.Sleep(1000);
        Console.WriteLine("Origin task must complete by this moment...");
        MyTask<int> task2 = originTask.ContinueWith(x => x * 3);
        MyTask<string> task3 = originTask.ContinueWith(x => $"{x} is cool number");

        Console.WriteLine($"Task1 result: {task1.Result}");
        Console.WriteLine($"Task2 result: {task2.Result}");
        Console.WriteLine($"Task3 result: {task3.Result}");
        Console.WriteLine();

        Console.WriteLine("Pipeline with exceptions:");

        int numOfTasks = 5;
        int taskWithException = 2;
        
        List<MyTask<int>> piplineTasks = new List<MyTask<int>>(numOfTasks);

        piplineTasks.Add(pool.StartNewTask(() =>
        {
            Thread.Sleep(300);
            return 10;
        }));
        
        for (int i = 1; i < numOfTasks; ++i)
        {
            if (i == taskWithException)
                piplineTasks.Add(piplineTasks[i - 1].ContinueWith<int>(_ => throw new Exception("Oops")));
            else
                piplineTasks.Add(piplineTasks[i - 1].ContinueWith(x =>
                {
                    Thread.Sleep(300);
                    return x + 1;
                }));
        }

        Thread.Sleep(taskWithException * 300 + 200);
        Console.WriteLine($"Task{taskWithException} must complete by this moment without exception in main thread");

        for (int i = 0; i < numOfTasks; ++i)
        {
            try
            {
                Console.WriteLine($"Task{i} result: {piplineTasks[i].Result}");
            }
            catch (AggregateException ae)
            {
                Console.WriteLine($"Task{i} exception:");
                Console.WriteLine(ae);
            }
        }
    }
}