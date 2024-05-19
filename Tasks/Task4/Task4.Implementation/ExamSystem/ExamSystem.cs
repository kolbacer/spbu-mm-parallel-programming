using Task4.Implementation.Common.Exceptions;
using Task4.Implementation.ConcurrentSet;
using Task4.Implementation.ConcurrentSet.LazySet;
using Task4.Implementation.ConcurrentSet.StripedHashSet;

namespace Task4.Implementation.ExamSystem;

/// <summary>
/// Concurrent exam system, supporting different set types.
/// </summary>
public class ExamSystem : IExamSystem
{
    private readonly IConcurrentSet<Credit> _set;
    public int Count => _set.Count;

    public ExamSystem(SetType setType)
    {
        _set = setType switch
        {
            SetType.LazySet => new LazySet<Credit>(),
            SetType.StripedHashSet => new StripedHashSet<Credit>(50),
            _ => throw new InvalidSetTypeException()
        };
    }

    public void Add(long studentId, long courseId)
    {
        _set.Add((studentId, courseId));
    }

    public void Remove(long studentId, long courseId)
    {
        _set.Remove((studentId, courseId));
    }

    public bool Contains(long studentId, long courseId)
    {
        return _set.Contains((studentId, courseId));
    }
}