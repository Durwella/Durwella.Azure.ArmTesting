using System;

namespace Durwella.Azure.ArmTesting.Build
{
    class Program
    {
        string CompanyName => Class1.Name;

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Hello from Durwella.Azure.ArmTesting.Build!");
#endif
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Durwella.Azure.ArmTesting.Build requires a command line argument of the project path.");
                return;
            }
            var projectPath = args[0];
#if DEBUG
            Console.WriteLine($"Project Path: {projectPath}");
#endif
        }
    }
}
