using Task1.Example.Cases;
using Task1.Implementation.Utils;

namespace Task1.Example;

internal class Program
{
    static void Main(string[] args)
    {
        MpiHelper.RunWithMpi(args, () =>
        {
            GraphUsage.Run();
            PrimSolverUsage.Run();
            AnalyzeLargeGraphs.Run();
        });
    }
}