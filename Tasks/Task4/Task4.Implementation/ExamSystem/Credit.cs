namespace Task4.Implementation.ExamSystem;

/// <summary>
/// Struct representing a student's credit for a course.
/// </summary>
public readonly struct Credit
{
    public long StudentId { get; }
    public long CourseId { get; }

    public Credit(long studentId, long courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
    }
    
    public static implicit operator Credit((long, long) value)
    {
        return new Credit(value.Item1, value.Item2);
    }

    public override bool Equals(object? obj)
    {
        return obj is Credit c && this == c;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StudentId, CourseId);
    }

    public static bool operator ==(Credit x, Credit y)
    {
        return x.StudentId == y.StudentId && x.CourseId == y.CourseId;
    }

    public static bool operator !=(Credit x, Credit y)
    {
        return !(x == y);
    }
}