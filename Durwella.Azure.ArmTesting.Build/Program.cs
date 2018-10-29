using System.Linq;
using static System.Console;

namespace Durwella.Azure.ArmTesting.Build
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            WriteLine("Hello from Durwella.Azure.ArmTesting.Build!");
#endif
            if (args.Length == 0)
            {
                Error.WriteLine("Durwella.Azure.ArmTesting.Build requires a command line argument of the project path.");
                return;
            }
            var projectPath = args[0];
#if DEBUG
            WriteLine($"Project Path: {projectPath}");
#endif
            Write("  ARM Templates: ");
            var armTemplateEnumeration = new ArmTesting.Services.ArmTemplateEnumeration();
            var armTemplates = armTemplateEnumeration.EnumerateArmTemplatePaths(projectPath);
            if (armTemplates.Any())
                WriteLine();
            else
                WriteLine("(none)");
            foreach (var armTemplate in armTemplates)
            {
                WriteLine($"  - {armTemplate}");
            }
        }
    }
}
