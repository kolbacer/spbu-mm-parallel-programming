using Task3.Implementation.MyTask;
using Task3.Implementation.ThreadPool;

namespace Task3.Example.Cases;

public static class EndlessWork
{
    private static volatile bool stop = false;
    
    public static void Run()
    {
        Console.WriteLine("//// Endless work example ////");
        Console.WriteLine();
        
        int numOfThreads = 8;
        int numOfPipelines = 10;
        int spawnCount = 10;
        int timeoutLower = 200;
        int timeoutUpper = 600;
        Random rand = new Random();
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);
        
        MyTask<int>[] tasksToWait = new MyTask<int>[numOfPipelines];
        MyTask<int>[] pipelines = new MyTask<int>[numOfPipelines];
        for (int i = 0; i < numOfPipelines; ++i)
        {
            pipelines[i] = pool.StartNewTask(() =>
            {
                Thread.Sleep(1000);
                return 0;
            });
        }

        Thread userInputThread = new Thread(() =>
        {
            Console.WriteLine("Press any key to stop");
            Console.ReadKey();
            stop = true;
        });
        userInputThread.Start();

        bool firstCycle = true;
        
        while (!stop)
        {
            if (!firstCycle)
                for (int i = 0; i < numOfPipelines; ++i)
                {
                    tasksToWait[i] = pipelines[i];
                }
            
            for (int i = 0; i < spawnCount; ++i)
            {
                for (int j = 0; j < numOfPipelines; ++j)
                {
                    int pipelineNumber = j;
                    pipelines[j] = pipelines[j].ContinueWith(_ =>
                    {
                        Console.WriteLine($"Task from pipeline {pipelineNumber} in thread {Environment.CurrentManagedThreadId}");
                        Thread.Sleep(rand.Next(timeoutLower, timeoutUpper));
                        return 0;
                    });
                }
            }
            
            if (!firstCycle)
                for (int i = 0; i < numOfPipelines; ++i)
                {
                    Console.WriteLine($"Waiting for task in pipeline {i}...");
                    while (!tasksToWait[i].IsCompleted && !tasksToWait[i].IsFailed)
                        Thread.Yield();
                }

            firstCycle = false;
        }
    }
}