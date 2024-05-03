using System.Diagnostics.CodeAnalysis;

namespace Task3.Implementation.Primitives.Queue;

public interface IQueue<T> : IStorage
{
    void Enqueue(T item);
    T Dequeue();
    bool TryDequeue([MaybeNullWhen(false)] out T item);
}