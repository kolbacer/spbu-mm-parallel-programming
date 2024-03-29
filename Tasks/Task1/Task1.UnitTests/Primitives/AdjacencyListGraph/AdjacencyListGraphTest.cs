using Task1.Implementation.Primitives.AdjacencyListGraph;
using Task1.Implementation.Utils;

namespace Task1.UnitTests.Primitives.AdjacencyListGraph;

public class AdjacencyListGraphTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void AdjacencyListGraphAddAndRemoveVertexTest()
    {
        AdjacencyListGraph<int, int> graph = new AdjacencyListGraph<int, int>();
        
        graph.AddVertex(1);
        Assert.True(graph.HasVertex(1));
        Assert.That(graph.VerticesCount, Is.EqualTo(1));

        graph.RemoveVertex(1);
        Assert.False(graph.HasVertex(1));
        Assert.That(graph.VerticesCount, Is.EqualTo(0));
    }
    
    [Test]
    public void AdjacencyListGraphAddAndRemoveEdgeTest()
    {
        AdjacencyListGraph<int, int> graph = new AdjacencyListGraph<int, int>();
        
        graph.AddEdge(1, 2, 555);
        Assert.True(graph.HasVertex(1));
        Assert.True(graph.HasVertex(2));
        Assert.True(graph.HasEdge(1, 2));
        Assert.That(graph.GetEdge(1, 2), Is.EqualTo(new Edge<int, int>(1, 2, 555)));
        Assert.That(graph.VerticesCount, Is.EqualTo(2));
        Assert.That(graph.EdgesCount, Is.EqualTo(1));

        graph.RemoveEdge(1, 2);
        Assert.True(graph.HasVertex(1));
        Assert.True(graph.HasVertex(2));
        Assert.False(graph.HasEdge(1, 2));
        Assert.That(graph.VerticesCount, Is.EqualTo(2));
        Assert.That(graph.EdgesCount, Is.EqualTo(0));
    }
    
    [Test]
    public void AdjacencyListGraphAddAndRemoveVertexWithEdgesTest()
    {
        AdjacencyListGraph<int, int> graph = new AdjacencyListGraph<int, int>();

        const int vertex = 999;
        
        graph.AddEdge(vertex, 1, 111);
        graph.AddEdge(vertex, 2, 222);
        graph.AddEdge(vertex, 3, 333);
        graph.AddEdge(4, vertex, 444);
        graph.AddEdge(5, vertex, 555);
        graph.AddEdge(6, vertex, 666);
        
        graph.AddEdge(1, 2, 12);
        graph.AddEdge(3, 4, 34);
        
        Assert.That(graph.VerticesCount, Is.EqualTo(7));
        Assert.That(graph.EdgesCount, Is.EqualTo(8));

        graph.RemoveVertex(vertex);
        
        Assert.False(graph.HasVertex(vertex));
        Assert.False(graph.HasEdge(vertex, 1));
        Assert.False(graph.HasEdge(vertex, 2));
        Assert.False(graph.HasEdge(vertex, 3));
        Assert.False(graph.HasEdge(4, vertex));
        Assert.False(graph.HasEdge(5, vertex));
        Assert.False(graph.HasEdge(6, vertex));
        Assert.True(graph.HasEdge(1, 2));
        Assert.True(graph.HasEdge(3, 4));
        Assert.That(graph.VerticesCount, Is.EqualTo(6));
        Assert.That(graph.EdgesCount, Is.EqualTo(2));
    }

    [Test]
    public void AdjacencyListGraphGetIncidentEdgesTest()
    {
        AdjacencyListGraph<int, int> graph = GraphBuilder.ReadFromString<int, int>("""
            4
            1 2 12
            1 3 13
            1 4 14
            3 4 35
            """);

        List<Edge<int, int>> subsetEdges = graph.GetIncidentEdges(1, new [] { 2, 3, 6 });
        List<Edge<int, int>> allEdges = graph.GetIncidentEdges(1);
        
        Assert.That(subsetEdges, Is.EqualTo(new Edge<int, int>[] {(1, 2, 12), (1, 3, 13)}));
        Assert.That(allEdges, Is.EqualTo(new Edge<int, int>[] {(1, 2, 12), (1, 3, 13), (1, 4, 14)}));
    }
    
    [Test]
    public void AdjacencyListGraphGetCutEdgesTest()
    {
        AdjacencyListGraph<int, int> graph = GraphBuilder.ReadFromString<int, int>("""
            4
            1 2 12
            1 3 13
            2 3 23
            2 4 24
            3 4 34
            """);

        List<Edge<int, int>> cutEdges = graph.GetCutEdges(new[] { 1, 2 }, new[] { 3, 4 });
        
        Assert.That(cutEdges, Is.EqualTo(new Edge<int, int>[] {(1, 3, 13), (2, 3, 23), (2, 4, 24)}));
    }

    [Test]
    public void AdjacencyListGraphCopyTest()
    {
        AdjacencyListGraph<int, int> original = new AdjacencyListGraph<int, int>();
        original.AddEdge(1, 2, 12);
        original.AddEdge(1, 3, 13);
        original.AddEdge(2, 3, 23);

        AdjacencyListGraph<int, int> copy = original.Copy();
        
        original.AddEdge(5, 6, 56);
        copy.RemoveEdge(2, 3);
        
        Assert.True(original.HasEdge(5, 6));
        Assert.False(copy.HasEdge(5, 6));
        Assert.True(original.HasEdge(2, 3));
        Assert.False(copy.HasEdge(2, 3));
        Assert.That(original.EdgesCount, Is.EqualTo(4));
        Assert.That(copy.EdgesCount, Is.EqualTo(2));
    }

}