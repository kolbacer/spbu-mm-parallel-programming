using Task1.Implementation.Primitives.AdjacencyListGraph;

namespace Task1.UnitTests.Primitives.AdjacencyListGraph;

public class EdgeTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void EdgeCompareTest()
    {
        Edge<int, int> edge1 = new Edge<int, int>(1, 2, 100);
        Edge<int, int> edge2 = new Edge<int, int>(2, 1, 100);
        Edge<int, int> edge3 = new Edge<int, int>(2, 3, 500, true);
        Edge<int, int> edge4 = new Edge<int, int>(3, 2, 500, true);
        
        Assert.True(edge1 == edge2);
        Assert.True(edge3 != edge4);

        Assert.True(edge1 == (1, 2, 100));
        Assert.True(edge1 == (2, 1, 100));
        Assert.True((1, 2, 100) == edge1);
        Assert.True((2, 1, 100) == edge1);
        Assert.True(edge1 != (2, 1, 555));
        Assert.True(edge1 != (5, 6, 100));
        Assert.True((2, 1, 555) != edge1);
        Assert.True((5, 6, 100) != edge1);
        
        Assert.True(edge3 == (2, 3, 500));
        Assert.True(edge3 != (3, 2, 500));
    }
}