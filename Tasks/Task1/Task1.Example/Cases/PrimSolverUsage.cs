using Task1.Implementation;
using Task1.Implementation.Primitives.AdjacencyListGraph;
using Task1.Implementation.Utils;

namespace Task1.Example.Cases;

public static class PrimSolverUsage
{
    public static void Run()
    {
        MpiHelper.PrintLine("#### Prim solver usage ####");

        AdjacencyListGraph<int, int> graph = GraphBuilder.ReadFromString<int, int>("""
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
            """);

        MpiHelper.PrintLine();
        MpiHelper.PrintLine("Graph:");
        MpiHelper.PrintLine(graph);

        AdjacencyListGraph<int, int> seqMst = PrimSolver.FindMst(graph);
        
        MpiHelper.PrintLine();
        MpiHelper.PrintLine("MST found by sequential algorithm:");
        MpiHelper.PrintLine(seqMst);
        
        AdjacencyListGraph<int, int> paralMst = PrimSolver.FindMstParallel(graph);

        MpiHelper.PrintLine();
        MpiHelper.PrintLine("MST found by parallel algorithm:");
        MpiHelper.PrintLine(paralMst);
    }
}