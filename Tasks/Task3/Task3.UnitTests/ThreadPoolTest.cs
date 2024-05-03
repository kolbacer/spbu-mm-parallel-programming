using Task3.Implementation.MyTask;
using Task3.Implementation.ThreadPool;

namespace Task3.UnitTests;

/// <summary>
/// Unit tests for <see cref="T:Task3.Implementation.ThreadPool.MyThreadPool"/>
/// and <see cref="T:Task3.Implementation.MyTask.MyTask`1"/>
/// </summary>
[Parallelizable(scope: ParallelScope.All)]
public class ThreadPoolTest
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    /// Test of creation and enqueuing single task
    /// </summary>
    [Test]
    public void SingleTaskAddingTest()
    {
        int numOfThreads = 4;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);

        MyTask<int> task = new MyTask<int>(() => 100);
        pool.Enqueue(task);

        Assert.That(task.Result, Is.EqualTo(100));
    }

    /// <summary>
    /// Test of creation and enqueuing multiple tasks
    /// </summary>
    [Test]
    public void MultipleTasksAddingTest()
    {
        int numOfThreads = 4;
        int numOfTasks = 100;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);
        
        List<MyTask<int>> tasks = Enumerable.Range(0, numOfTasks)
            .Select<int, MyTask<int>>(i => new MyTask<int>(() => i)).ToList();
        foreach (var task in tasks)
            pool.Enqueue(task);

        for (int i = 0; i < numOfTasks; ++i)
            Assert.That(tasks[i].Result, Is.EqualTo(i));
    }

    /// <summary>
    /// Test of continuation tasks in pipeline
    /// </summary>
    [Test]
    public void ContinuationPipelineTest()
    {
        int numOfThreads = 4;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);
        
        int initValue = 0;
        MyTask<int> pipeline = pool
            .StartNewTask(() => initValue)
            .ContinueWith(x => x + 5)
            .ContinueWith(x => x * 10)
            .ContinueWith(x => x - 13)
            .ContinueWith(x => x + 33);
        
        Assert.That(pipeline.Result, Is.EqualTo(70));
    }
    
    /// <summary>
    /// Test of continuation multiple tasks to the same origin task
    /// </summary>
    [Test]
    public void ContinuationMultipleTest()
    {
        int numOfThreads = 4;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);

        MyTask<int> originTask = pool.StartNewTask(() => 5);
        
        MyTask<int> task1 = originTask.ContinueWith(x => x * 2);
        MyTask<int> task2 = originTask.ContinueWith(x => x * 3);
        MyTask<string> task3 = originTask.ContinueWith(x => $"#{x}#");
        
        Assert.That(task1.Result, Is.EqualTo(10));
        Assert.That(task2.Result, Is.EqualTo(15));
        Assert.That(task3.Result, Is.EqualTo("#5#"));
    }
    
    /// <summary>
    /// Test of exceptions in task continuation
    /// </summary>
    [Test]
    public void ContinuationExceptionTest()
    {
        int numOfThreads = 4;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);

        MyTask<int> task1 = pool.StartNewTask(() => 1);
        MyTask<int> task2 = task1.ContinueWith(x => x + 2);
        MyTask<int> task3 = task2.ContinueWith<int>(x => throw new Exception("oops"));
        MyTask<int> task4 = task3.ContinueWith(x => x + 4);
        MyTask<int> task5 = task4.ContinueWith(x => x + 5);
        
        Assert.That(task1.Result, Is.EqualTo(1));
        Assert.That(task2.Result, Is.EqualTo(3));
        Assert.Throws<AggregateException>(() => { int x = task3.Result; });
        Assert.Throws<AggregateException>(() => { int x = task4.Result; });
        Assert.Throws<AggregateException>(() => { int x = task5.Result; });
    }
    
    /// <summary>
    /// Test of thread count in thread pool
    /// </summary>
    [Test]
    public void NumberOfThreadsTest()
    {
        int numOfThreads = 4;
        WorkStrategy strategy = WorkStrategy.WorkSharing;
        using MyThreadPool pool = new MyThreadPool(numOfThreads, strategy);

        List<int> threadIds = pool.Threads.Values.Select(thread => thread.ManagedThreadId).ToList();
        
        Assert.That(threadIds.Count, Is.EqualTo(numOfThreads));
        Assert.That(threadIds.Distinct().Count(), Is.EqualTo(threadIds.Count));
    }
}