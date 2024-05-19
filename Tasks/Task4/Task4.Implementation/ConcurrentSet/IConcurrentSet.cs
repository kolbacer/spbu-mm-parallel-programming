namespace Task4.Implementation.ConcurrentSet;

public interface IConcurrentSet<in T>
{
    public int Count { get; }
    public bool Add(T item);
    public bool Remove(T item);
    public bool Contains(T item);
}