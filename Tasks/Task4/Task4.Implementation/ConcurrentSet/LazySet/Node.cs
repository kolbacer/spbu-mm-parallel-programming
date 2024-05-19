namespace Task4.Implementation.ConcurrentSet.LazySet;

/// <summary>
/// <see cref="T:Task4.Implementation.ConcurrentSet.LazySet.LazySet`1"/> node.
/// Based on Common/LocksContinued/Interfaces/Node.
/// </summary>
/// <typeparam name="T">Item type</typeparam>
internal class Node<T>
{
    public int Key { get; }
    public T? Value { get; }
    public Node<T>? Next { get; set; }
    public object Locker { get; } = new();
    public bool Marked { get; set; } = false;

    public Node(int key)
    {
        Key = key;
    }

    public Node(T value)
    {
        Key = (value == null) ? 0 : value.GetHashCode();
        Value = value;
    }

    public override int GetHashCode()
    {
        return Key;
    }
}