# Task1

Parallel implementation of Prim's algorithm with MPI technology in C# (using MPI.NET).

## Prerequisites
- `.NET 7.0 SDK`
- `MPI` (`mpiexec` must be in environment)
- `MPI.NET`
- `NUnit`

## Structure
```
.
├── README.md - task description
├── Task1.Example - project with examples of using the algorithm and primitives
│   ├── Cases/ - use cases
│   └── TestFiles/ - files with graphs and the results of their processing
├── Task1.Implementation - project containing an implementation of the algorithm and related entities
└── Task1.UnitTests - project containing unit tests for algorithm and primitives
```