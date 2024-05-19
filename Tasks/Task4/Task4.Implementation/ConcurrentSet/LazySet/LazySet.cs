using Task4.Implementation.Common.Exceptions;

namespace Task4.Implementation.ConcurrentSet.LazySet;

/// <summary>
/// Concurrent set based on Common/LocksContinued/Sets/4_LazySet.
/// Stores items in a singly linked list, up to their hashcode.
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class LazySet<T> : IConcurrentSet<T>
{
    private readonly Node<T> _head = new(int.MinValue);
    private readonly Node<T> _tail = new(int.MaxValue);
    private volatile int _count = 0;
    public int Count => _count;

    public LazySet()
    {
        _head.Next = _tail;
    }

    /// <summary>
    /// Check that adjacent elements are nearby and not removed.
    /// </summary>
    private bool Validate(Node<T> pred, Node<T> curr)
    {
        return !pred.Marked && !curr.Marked && pred.Next == curr;
    }

    /// <summary>
    /// Add new item to set. Not starvation-free.
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>True if the method added the item. False if the item has been added previously.</returns>
    /// <exception cref="InvalidHashCodeException">Throws if the item has a reserved hashcode.</exception>
    public bool Add(T item)
    {
        int key = (item == null) ? 0 : item.GetHashCode();
        if (key == _head.Key || key == _tail.Key)
            throw new InvalidHashCodeException("Item must not have a hashcode equal to int.MinValue or int.MaxValue");
        
        while (true)
        {
            Node<T> pred = _head;
            Node<T> curr = _head.Next!;
            while (curr!.Key < key)
            {
                pred = curr;
                curr = curr.Next!;
            }

            lock (pred.Locker)
            {
                lock (curr.Locker)
                {
                    if (Validate(pred, curr))
                    {
                        if (curr.Key == key)
                            return false;
                        Node<T> node = new Node<T>(item) { Next = curr };
                        pred.Next = node;
                        Interlocked.Increment(ref _count);
                        return true;
                    }   
                }
            }
        }
    }

    /// <summary>
    /// Remove item from set. Not starvation-free.
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns>True if the method removed the item. False if the element was not in the set.</returns>
    /// <exception cref="InvalidHashCodeException">Throws if the item has a reserved hashcode.</exception>
    public bool Remove(T item)
    {
        int key = (item == null) ? 0 : item.GetHashCode();
        if (key == _head.Key || key == _tail.Key)
            throw new InvalidHashCodeException("Item must not have a hashcode equal to int.MinValue or int.MaxValue");
        
        while (true)
        {
            Node<T> pred = _head;
            Node<T> curr = _head.Next!;
            while (curr!.Key < key)
            {
                pred = curr;
                curr = curr.Next!;
            }

            lock (pred.Locker)
            {
                lock (curr.Locker)
                {
                    if (Validate(pred, curr))
                    {
                        if (curr.Key != key)
                            return false;
                        curr.Marked = true;
                        pred.Next = curr.Next;
                        Interlocked.Decrement(ref _count);
                        return true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check if a set contains an item. The result may not be relevant by the return moment.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <returns>True if contains, False otherwise.</returns>
    /// <exception cref="InvalidHashCodeException">Throws if the item has a reserved hashcode.</exception>
    public bool Contains(T item)
    {
        int key = (item == null) ? 0 : item.GetHashCode();
        if (key == _head.Key || key == _tail.Key)
            throw new InvalidHashCodeException("Item must not have a hashcode equal to int.MinValue or int.MaxValue");
        
        Node<T> curr = _head;
        while (curr!.Key < key)
            curr = curr.Next!;
        return curr.Key == key && !curr.Marked;
    }
}