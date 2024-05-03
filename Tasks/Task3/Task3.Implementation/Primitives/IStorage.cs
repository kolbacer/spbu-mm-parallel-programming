namespace Task3.Implementation.Primitives;

public interface IStorage
{
    int Count { get; }
    bool IsEmpty { get; }
}