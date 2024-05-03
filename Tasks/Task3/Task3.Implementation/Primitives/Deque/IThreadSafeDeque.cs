using System.Diagnostics.CodeAnalysis;

namespace Task3.Implementation.Primitives.Deque;

public interface IThreadSafeDeque<T>: IDeque<T>
{
    object Locker { get; }
    int UnsafeCount { get; }
    bool UnsafeIsEmpty { get; }
    
    void UnsafePushTop(T value);
    void UnsafePushBottom(T value);
    
    T UnsafePopTop();
    T UnsafePopBottom();

    bool UnsafeTryPopTop([MaybeNullWhen(false)] out T item);
    bool UnsafeTryPopBottom([MaybeNullWhen(false)] out T item);
}