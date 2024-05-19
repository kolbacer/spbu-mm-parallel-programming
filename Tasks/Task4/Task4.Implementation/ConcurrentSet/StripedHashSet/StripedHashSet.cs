using Task4.Implementation.Common.Exceptions;

namespace Task4.Implementation.ConcurrentSet.StripedHashSet;

/// <summary>
/// Concurrent set based on Common/LocksContinued/Hashing/2_StripedHashSet.
/// Stores items in buckets (hashset).
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class StripedHashSet<T>: IConcurrentSet<T>
{
    private List<T>[] _table;
    private readonly Mutex[] _locks;
    private bool PolicyDemandsResize => _count / _table.Length > 4;
    
    private volatile int _count = 0;
    public int Count => _count;
    
    /// <summary>
    /// Create new StripedHashSet with initial capacity of internal table and mutex array.
    /// The number of mutexes will remain unchanged throughout the existence of the StripedHashSet.
    /// </summary>
    /// <param name="capacity">Initial capacity</param>
    /// <exception cref="InvalidCapacityException">Throws if capacity is less than 1</exception>
    public StripedHashSet(int capacity)
    {
        if (capacity < 1)
            throw new InvalidCapacityException("Capacity must be greater than 0");
        
        _count = 0;
        _table = new List<T>[capacity];
        _locks = new Mutex[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _table[i] = new List<T>();
            _locks[i] = new Mutex();
        }
    }

    private int GetBucket(T item)
    {
        return (item == null) ? 0 : Math.Abs(item.GetHashCode() % _locks.Length);
    }

    private void Acquire(T item)
    {
        _locks[GetBucket(item)].WaitOne();
    }

    private void Release(T item)
    {
        _locks[GetBucket(item)].ReleaseMutex();
    }

    private void Resize()
    {
        int oldCapacity = _table.Length;

        foreach (Mutex m in _locks)
        {
            m.WaitOne();
        }

        try
        {
            if (oldCapacity != _table.Length)
                return; // someone beat us to it
            int newCapacity = 2 * oldCapacity;
            List<T>[] oldTable = _table;
            _table = new List<T>[newCapacity];
            for (int i = 0; i < newCapacity; i++)
                _table[i] = new List<T>();
            foreach (List<T> bucket in oldTable)
            {
                foreach (T x in bucket)
                {
                    _table[GetBucket(x)].Add(x);
                }
            }
        }
        finally
        {
            foreach (Mutex m in _locks)
            {
                m.ReleaseMutex();
            }
        }
    }
    
    /// <summary>
    /// Add new item to set.
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>True if the method added the item. False if the item has been added previously.</returns>
    public bool Add(T item)
    {
        bool result = false;
        Acquire(item);
        try
        {
            if (!_table[GetBucket(item)].Contains(item))
            {
                _table[GetBucket(item)].Add(item);
                result = true;
                Interlocked.Increment(ref _count);
            }
        }
        finally
        {
            Release(item);
        }
        if (PolicyDemandsResize)
            Resize();
        return result;
    }
    
    /// <summary>
    /// Remove item from set.
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns>True if the method removed the item. False if the element was not in the set.</returns>
    public bool Remove(T item)
    {
        Acquire(item);
        try
        {
            bool result = _table[GetBucket(item)].Remove(item);
            if (result) Interlocked.Decrement(ref _count);
            return result;
        }
        finally
        {
            Release(item);
        }
    }
    
    /// <summary>
    /// Check if a set contains an item.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <returns>True if contains, False otherwise.</returns>
    public bool Contains(T item)
    {
        Acquire(item);
        try
        {
            return _table[GetBucket(item)].Contains(item);
        }
        finally
        {
            Release(item);
        }
    }
}