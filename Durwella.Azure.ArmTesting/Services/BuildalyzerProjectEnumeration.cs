using Buildalyzer;
using Durwella.Azure.ArmTesting.ServiceInterfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Durwella.Azure.ArmTesting.Services
{
    public class BuildalyzerProjectEnumeration : IProjectFileEnumeration
    {
        private static readonly IList<string> InlcudedProjectKeys =
            new[] { "None", "Compile", "Content", "EmbeddedResource" };

        public IEnumerable<string> EnumerateProjectFiles(string projectPath)
        {
            var relativePaths = EnumerateProjectFilesRelative(projectPath);
            var directory = Path.GetDirectoryName(projectPath);
            return relativePaths.Select(relativePath => Path.Combine(directory, relativePath));
        }

        public IEnumerable<string> EnumerateProjectFilesRelative(string projectPath)
        {
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(projectPath);
            var results = analyzer.Build();
            var result = results.Single();
            var items = result.Items;
            foreach (var item in items)
            {
                // Skip keys like ProjectReference that aren't for files
                if (!InlcudedProjectKeys.Contains(item.Key))
                    continue;
                var projectItems = item.Value;
                foreach (var projectItem in projectItems)
                {
                    // The item spec for files will be the path relative to the project directory
                    yield return projectItem.ItemSpec;
                }
            }
        }
    }
}
