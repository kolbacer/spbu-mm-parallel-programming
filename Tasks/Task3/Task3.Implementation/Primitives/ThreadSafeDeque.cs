using System.Diagnostics.CodeAnalysis;
using Task3.Implementation.Primitives.Deque;
using Task3.Implementation.Primitives.Queue;

namespace Task3.Implementation.Primitives;

/// <summary>
/// Thread safe deque based on <see cref="T:System.Collections.Generic.LinkedList`1"/>.
/// Locks are used for synchronization.
/// </summary>
/// <typeparam name="T">Element type</typeparam>
public class ThreadSafeDeque<T>: IThreadSafeDeque<T>, IThreadSafeQueue<T>
{
    private LinkedList<T> Storage { get; }
    public object Locker { get; } = new();

    public int Count
    {
        get
        {
            lock (Locker)
            {
                return UnsafeCount;
            }
        }
    }
    
    public bool IsEmpty
    {
        get
        {
            lock (Locker)
            {
                return UnsafeIsEmpty;
            }
        }
    }

    public int UnsafeCount => Storage.Count;
    
    public bool UnsafeIsEmpty => Storage.Count == 0;

    public ThreadSafeDeque()
    {
        Storage = new LinkedList<T>();
    }

    public ThreadSafeDeque(IEnumerable<T> collection)
    {
        Storage = new LinkedList<T>(collection);
    }
    
    // safe methods

    public void PushTop(T value)
    {
        lock (Locker)
        {
            UnsafePushTop(value);
        }
    }
    
    public void PushBottom(T value)
    {
        lock (Locker)
        {
            UnsafePushBottom(value);
        }
    }
    
    public T PopTop()
    {
        lock (Locker)
        {
            return UnsafePopTop();
        }
    }
    
    public T PopBottom()
    {
        lock (Locker)
        {
            return UnsafePopBottom();
        }
    }
    
    public bool TryPopTop([MaybeNullWhen(false)] out T item)
    {
        lock (Locker)
        {
            return UnsafeTryPopTop(out item);
        }
    }
    
    public bool TryPopBottom([MaybeNullWhen(false)] out T item)
    {
        lock (Locker)
        {
            return UnsafeTryPopBottom(out item);
        }
    }
    
    public void Enqueue(T item) => PushTop(item);
    
    public T Dequeue() => PopBottom();
    
    public bool TryDequeue([MaybeNullWhen(false)] out T item) => TryPopBottom(out item);
    
    // unsafe methods
    
    public void UnsafePushTop(T value)
    {
        Storage.AddLast(value);
    }
    
    public void UnsafePushBottom(T value)
    {
        Storage.AddFirst(value);
    }
    
    public T UnsafePopTop()
    {
        if (Storage.Last == null)
            throw new InvalidOperationException("No top element");
        T item = Storage.Last.Value;
        Storage.RemoveLast();
        return item;
    }
    
    public T UnsafePopBottom()
    {
        if (Storage.First == null)
            throw new InvalidOperationException("No bottom element");
        T item = Storage.First.Value;
        Storage.RemoveFirst();
        return item;
    }
    
    public bool UnsafeTryPopTop([MaybeNullWhen(false)] out T item)
    {
        if (Storage.Last == null)
        {
            item = default;
            return false;
        }

        item = Storage.Last.Value;
        Storage.RemoveLast();
        return true;
    }
    
    public bool UnsafeTryPopBottom([MaybeNullWhen(false)] out T item)
    {
        if (Storage.First == null)
        {
            item = default;
            return false;
        }

        item = Storage.First.Value;
        Storage.RemoveFirst();
        return true;
    }
    
    public void UnsafeEnqueue(T item) => UnsafePushTop(item);
    
    public T UnsafeDequeue() => UnsafePopBottom();
    
    public bool UnsafeTryDequeue([MaybeNullWhen(false)] out T item) => UnsafeTryPopBottom(out item);

}