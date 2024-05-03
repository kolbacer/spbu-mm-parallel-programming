using System.Diagnostics.CodeAnalysis;

namespace Task3.Implementation.Primitives.Deque;

public interface IDeque<T> : IStorage
{
    void PushTop(T value);
    void PushBottom(T value);
    
    T PopTop();
    T PopBottom();

    bool TryPopTop([MaybeNullWhen(false)] out T item);
    bool TryPopBottom([MaybeNullWhen(false)] out T item);

}