﻿namespace Task4.Implementation.ExamSystem;

public interface IExamSystem
{
    public int Count { get; }
    public void Add(long studentId, long courseId);
    public void Remove(long studentId, long courseId);
    public bool Contains(long studentId, long courseId);
}