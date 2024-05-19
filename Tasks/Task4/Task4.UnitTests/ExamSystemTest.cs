using Task4.Implementation.ExamSystem;

namespace Task4.UnitTests;

/// <summary>
/// Unit tests for <see cref="T:Task4.Implementation.ExamSystem.ExamSystem"/> and
/// sets: <see cref="T:Task4.Implementation.ConcurrentSet.LazySet.LazySet`1"/>,
/// <see cref="T:Task4.Implementation.ConcurrentSet.StripedHashSet.StripedHashSet`1"/>.
/// </summary>
[Parallelizable(scope: ParallelScope.All)]
public class ExamSystemTest
{
    
    public static readonly SetType[] SetTypes =
    {
        SetType.LazySet,
        SetType.StripedHashSet
    };
    
    [SetUp]
    public void Setup()
    {
    }

    [TestCaseSource(nameof(SetTypes))]
    public void SingleCreditTest(SetType setType)
    {
        ExamSystem examSystem = new ExamSystem(setType);
        Assert.False(examSystem.Contains(1, 1));
        Assert.That(examSystem.Count, Is.EqualTo(0));
        examSystem.Add(1, 1);
        Assert.True(examSystem.Contains(1, 1));
        Assert.That(examSystem.Count, Is.EqualTo(1));
        examSystem.Remove(1, 1);
        Assert.False(examSystem.Contains(1, 1));
        Assert.That(examSystem.Count, Is.EqualTo(0));
    }

    [TestCaseSource(nameof(SetTypes))]
    public void MultipleCreditsTest(SetType setType)
    {
        int count = 100;
        List<int> studentIds = Enumerable.Range(1, count).ToList();
        List<int> courseIds = Enumerable.Range(1, count).Select(x => x * 2).Reverse().ToList();

        ExamSystem examSystem = new ExamSystem(setType);
        
        for (int i = 0; i < count; ++i)
        {
            examSystem.Add(studentIds[i], courseIds[i]);
            // try duplicate
            if (i % 3 == 0)
                examSystem.Add(studentIds[i], courseIds[i]);
        }
        
        Assert.That(examSystem.Count, Is.EqualTo(count));
        
        for (int i = 0; i < count; ++i)
            Assert.True(examSystem.Contains(studentIds[i], courseIds[i]));

        for (int i = 0; i < count; i+=3)
            examSystem.Remove(studentIds[i], courseIds[i]);
        
        for (int i = 0; i < count; ++i)
        {
            bool contains = examSystem.Contains(studentIds[i], courseIds[i]);
            if (i % 3 == 0)
                Assert.False(contains);
            else 
                Assert.True(contains);
        }
    }
    
    [TestCaseSource(nameof(SetTypes))]
    public void MultipleCreditsParallelTest(SetType setType)
    {
        int count = 100;
        List<int> studentIds = Enumerable.Range(1, count).ToList();
        List<int> courseIds = Enumerable.Range(1, count).Select(x => x * 2).Reverse().ToList();

        ExamSystem examSystem = new ExamSystem(setType);

        Parallel.For(0, count, i =>
        {
            examSystem.Add(studentIds[i], courseIds[i]);
            // try duplicate
            if (i % 3 == 0)
                examSystem.Add(studentIds[i], courseIds[i]);
        });
        
        Assert.That(examSystem.Count, Is.EqualTo(count));
        
        for (int i = 0; i < count; ++i)
            Assert.True(examSystem.Contains(studentIds[i], courseIds[i]));

        Parallel.ForEach(Enumerable.Range(0, count / 3 + 1).Select(i => i * 3), i =>
            examSystem.Remove(studentIds[i], courseIds[i]));
        
        for (int i = 0; i < count; ++i)
        {
            bool contains = examSystem.Contains(studentIds[i], courseIds[i]);
            if (i % 3 == 0)
                Assert.False(contains);
            else 
                Assert.True(contains);
        }
    }
}