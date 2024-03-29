using Task1.Implementation.Primitives.AdjacencyListGraph;
using Task1.Implementation.Utils;

namespace Task1.Example.Cases;

public static class GraphUsage
{
    public static void Run()
    {
        MpiHelper.PrintLine("#### Graph usage ####");

        string projectDirectory = Directory.GetParent(System.Environment.CurrentDirectory).Parent.Parent.FullName;
        string graphDirectory = $"{projectDirectory}\\TestFiles\\";
        string graphFileName = "TestGraph.txt";
        
        MpiHelper.PrintLine();
        MpiHelper.PrintLine("Read graph from file:");

        AdjacencyListGraph<int, int> graph1 = null;
        if (MPI.Communicator.world.Rank == 0) 
            graph1 = GraphBuilder.ReadFromFile<int, int>(graphDirectory + graphFileName);
        MpiHelper.PrintLine(graph1);
        
        string graphString = """
            3
            1 4 33
            1 8 55
            4 6 99
            5 7 1050
            """;
        
        MpiHelper.PrintLine();
        MpiHelper.PrintLine("Read graph from string:");
        
        AdjacencyListGraph<int, int> graph2 = GraphBuilder.ReadFromString<int, int>(graphString);
        MpiHelper.PrintLine(graph2);

        AdjacencyListGraph<int, int> graphCopy = null;
        if (MPI.Communicator.world.Rank == 0)
        {
            graphCopy = graph1.Copy();
            graph1.AddEdge((1, 10, 999));
            graphCopy.RemoveEdge(1, 2);   
        }

        MpiHelper.PrintLine();
        MpiHelper.PrintLine("Graph copying");
        MpiHelper.PrintLine("Original:");
        MpiHelper.PrintLine(graph1);
        MpiHelper.PrintLine();
        MpiHelper.PrintLine("Copy:");
        MpiHelper.PrintLine(graphCopy);
    }
}