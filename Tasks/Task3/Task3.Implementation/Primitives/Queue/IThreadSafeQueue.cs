using System.Diagnostics.CodeAnalysis;

namespace Task3.Implementation.Primitives.Queue;

public interface IThreadSafeQueue<T>: IQueue<T>
{
    object Locker { get; }
    int UnsafeCount { get; }
    bool UnsafeIsEmpty { get; }
    
    void UnsafeEnqueue(T item);
    T UnsafeDequeue();
    bool UnsafeTryDequeue([MaybeNullWhen(false)] out T item);
}