using System.Diagnostics;
using MPI;
using Task1.Implementation;
using Task1.Implementation.Primitives.AdjacencyListGraph;
using Task1.Implementation.Utils;

namespace Task1.Example.Cases;

public static class AnalyzeLargeGraphs
{
    public static void Run()
    {
        MpiHelper.PrintLine("#### Analyze large graphs ####");
        
        string projectDirectory = Directory.GetParent(System.Environment.CurrentDirectory).Parent.Parent.FullName;
        string graphDirectory = $"{projectDirectory}\\TestFiles\\Graphs\\";
        string resultDirectory = $"{projectDirectory}\\TestFiles\\Results\\";
        
        string graphFileName = "graph_5000_0.06.txt";

        Intracommunicator comm = MPI.Communicator.world;
        
        Stopwatch stopWatch = new Stopwatch();

        stopWatch.Start();
        MpiHelper.PrintLine();
        MpiHelper.Print($"Generating graph {graphFileName}... ");
        AdjacencyListGraph<int, int> graph1 = new AdjacencyListGraph<int, int>();
        if (comm.Rank == 0) 
            graph1 = GraphBuilder.GenerateRandomInt(5000, 0.06, 555);
        stopWatch.Stop();
        MpiHelper.PrintLine($"Done! Edges: {graph1.EdgesCount} ({stopWatch.Elapsed})");
        stopWatch.Reset();

        MpiHelper.Print($"Writing graph {graphFileName}... ");
        stopWatch.Start();
        if (comm.Rank == 0) 
            graph1.WriteToFile(graphDirectory + graphFileName);
        stopWatch.Stop();
        MpiHelper.PrintLine($"Done! ({stopWatch.Elapsed})");
        stopWatch.Reset();
        
        MpiHelper.Print($"Reading graph {graphFileName}... ");
        stopWatch.Start();
        AdjacencyListGraph<int, int> graph = new AdjacencyListGraph<int, int>(); 
        if (comm.Rank == 0) 
            graph = GraphBuilder.ReadFromFile<int, int>(graphDirectory + graphFileName);
        stopWatch.Stop();
        MpiHelper.PrintLine($"Done! ({stopWatch.Elapsed})");
        stopWatch.Reset();

        if (comm.Rank == 0)
        {
            MpiHelper.Print($"Finding MST of {graphFileName} (sequentially)... ");
            stopWatch.Start();
            AdjacencyListGraph<int, int> mstSeq = PrimSolver.FindMst(graph, true);
            stopWatch.Stop();
            MpiHelper.PrintLine($"Done! ({stopWatch.Elapsed})");
            stopWatch.Reset();
        
            MpiHelper.Print($"Counting total MST weight of {graphFileName}... ");
            int weightSeq = GetTotalWeight(mstSeq);
            MpiHelper.PrintLine($"Done! weight={weightSeq} ({stopWatch.Elapsed})");
            File.WriteAllText(resultDirectory + graphFileName, $"{mstSeq.VerticesCount}\n{weightSeq}");
            stopWatch.Reset();   
        }
        comm.Barrier();
        
        MpiHelper.Print($"Finding MST of {graphFileName} (parallel)... ");
        stopWatch.Start();
        AdjacencyListGraph<int, int> mstParal = PrimSolver.FindMstParallel(graph, true);
        stopWatch.Stop();
        MpiHelper.PrintLine($"Done! ({stopWatch.Elapsed})");
        stopWatch.Reset();
        
        MpiHelper.Print($"Counting total MST weight of {graphFileName}... ");
        int weightParal = 0;
        if (comm.Rank == 0) 
            weightParal = GetTotalWeight(mstParal);
        MpiHelper.PrintLine($"Done! weight={weightParal} ({stopWatch.Elapsed})");
        File.WriteAllText(resultDirectory + "mst_" + graphFileName, $"{mstParal.VerticesCount}\n{weightParal}");
        stopWatch.Reset();
    }

    public static int GetTotalWeight(AdjacencyListGraph<int, int> graph)
    {
        int sum = 0;
        foreach (var edgeDict in graph.Storage)
        {
            foreach (var edge in edgeDict.Value)
            {
                sum += edge.Value;
            }
        }

        return sum;
    }
}