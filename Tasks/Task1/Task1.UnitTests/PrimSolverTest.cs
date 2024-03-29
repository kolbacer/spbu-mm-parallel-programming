using MPI;
using Task1.Implementation;
using Task1.Implementation.Primitives.AdjacencyListGraph;
using Task1.Implementation.Utils;

namespace Task1.UnitTests;

public class PrimSolverTest
{
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    /// Tests for sequential implementation of Prim's algorithm
    /// </summary>
    [TestCaseSource(nameof(TestGraphs))]
    public void PrimSolverFindMstTest((AdjacencyListGraph<int, int> graph, int mstWeight) testCase)
    {
        AdjacencyListGraph<int, int> graph = testCase.graph;
        int mstWeight = testCase.mstWeight;

        AdjacencyListGraph<int, int> MST = PrimSolver.FindMst(graph);
        int weight = 0;
        foreach (var edgeDict in MST.Storage)
        {
            foreach (var edge in edgeDict.Value)
            {
                weight += edge.Value;
            }
        }
        
        Assert.That(weight, Is.EqualTo(mstWeight));
    }
    
    /// <summary>
    /// Tests for parallel implementation of Prim's algorithm.
    /// Must be run with MPI.
    /// </summary>
    [TestCaseSource(nameof(TestGraphs))]
    [Ignore("Needs MPI environment")]
    public void PrimSolverFindMstParallelTest((AdjacencyListGraph<int, int> graph, int mstWeight) testCase)
    {
        AdjacencyListGraph<int, int> graph = testCase.graph;
        int mstWeight = testCase.mstWeight;

        MpiHelper.RunWithMpi(Array.Empty<String>(), () =>
        {
            Intracommunicator comm = MpiHelper.GetWorldCommunicator();
            AdjacencyListGraph<int, int> MST = PrimSolver.FindMstParallel(graph);

            if (comm.Rank != 0) return;
            
            int weight = 0;
            foreach (var edgeDict in MST.Storage)
            {
                foreach (var edge in edgeDict.Value)
                {
                    weight += edge.Value;
                }
            }
            
            Assert.That(weight, Is.EqualTo(mstWeight));
        });
    }
    
    public static object[] TestGraphs =
    {
        (GraphBuilder.ReadFromString<int, int>("""
                5
                0 3 13
                0 1 28
                0 4 22
                0 2 13
                3 1 13
                3 4 19
                3 2 19
                1 4 13
                1 2 27
                4 2 14
                """), 52),
        (GraphBuilder.ReadFromString<int, int>("""
                5
                0 1 9
                0 2 75
                1 2 95
                1 3 19
                1 4 42
                2 3 51
                3 4 31 
                """), 110)
    };
}