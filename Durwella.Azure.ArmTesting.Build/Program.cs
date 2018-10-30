using Durwella.Azure.ArmTesting.ServiceInterfaces;
using Durwella.Azure.ArmTesting.Services;
using SimpleInjector;
using System.IO;
using System.Linq;
using static System.Console;

namespace Durwella.Azure.ArmTesting.Build
{
    static class Program
    {
        static readonly Container _container;

        static Program()
        {
            _container = new Container();
            _container.Register<IArmTemplateEnumeration, ArmTemplateEnumeration>();
        }

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
            var armTemplateEnumeration = _container.GetInstance<IArmTemplateEnumeration>();
            var armTemplates = armTemplateEnumeration.EnumerateArmTemplatePaths(projectPath);
            if (armTemplates.Any())
                WriteLine();
            else
                WriteLine("(none)");
            foreach (var armTemplate in armTemplates)
            {
                WriteLine($"  - {armTemplate}");
                // HACK: Quick wire up of name checking
                var nameChecking = new NameChecking();
                var json = File.ReadAllText(armTemplate);
                var errors = nameChecking.CheckTemplate(json)
                    .Select(e => new ArmTemplateError(armTemplate, e));
                foreach (var error in errors)
                {
                    WriteLine(error);
                }
            }
        }
    }
}
