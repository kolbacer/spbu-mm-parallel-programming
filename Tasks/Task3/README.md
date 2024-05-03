# Task3

ThreadPool implementation in C# with support of **Work Stealing** and **Work Sharing** scheduling strategies and task continuation.

## Prerequisites
- `.NET 7.0 SDK`
- `NUnit`

## Structure
```
.
├── README.md - task description
├── Task3.Example - project with examples of using MyThreadPool and MyTask
│   └── Cases/ - use cases
├── Task3.Implementation - project containing an implementation of MyThreadPool and related entities
│   ├── Common/ - general purpose classes
│   ├── Primitives/ - auxiliary data structures
│   ├── MyTask/ - implementation of MyTask
│   └── ThreadPool/ - implementation of MyThreadPool and scheduling algorithms
└── Task3.UnitTests - project containing unit tests for MyThreadPool and MyTask
```