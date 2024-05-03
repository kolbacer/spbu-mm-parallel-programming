using Task3.Example.Cases;

namespace Task3.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WorkStrategies.Run();
            Continuation.Run();
            // EndlessWork.Run();
        }
    }
}