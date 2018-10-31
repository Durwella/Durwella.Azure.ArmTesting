using Buildalyzer;
using Buildalyzer.Environment;
using Durwella.Azure.ArmTesting.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Durwella.Azure.ArmTesting.Services
{
    /// <summary>
    /// Uses Buildalyzer package to do a build and get all Items from the .csproj
    /// </summary>
    public class BuildalyzerProjectEnumeration : IProjectFileEnumeration
    {
        private static readonly IList<string> InlcudedProjectKeys =
            new[] { "None", "Compile", "Content", "EmbeddedResource" };

        public IEnumerable<string> EnumerateProjectFiles(string projectPath)
        {
            var relativePaths = EnumerateProjectFilesRelative(projectPath);
            var directory = Path.GetDirectoryName(projectPath);
            return relativePaths.Select(relativePath =>
                FullPath(relativePath, directory));
        }

        private static string FullPath(string relativePath, string directory)
        {
            return Path.GetFullPath(
                Path.Combine(directory, relativePath));
        }

        public IEnumerable<string> EnumerateProjectFilesRelative(string projectPath)
        {
            // Use an environment variable to try to prevent recursively calling this
            // build target when we call .Build below
            const string EnvVar = "_DRWLA_ARM_BUILD";
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvVar)))
            {
                yield break;
            }
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(projectPath);
            var environmentOptions = new EnvironmentOptions
            {
                // MUST USE DESIGN TIME BUILD
                // In the .targets definition we have a condition
                // to avoid running the DurwellaAzureArmTestingTarget at Design Time
                // https://github.com/dotnet/project-system/blob/master/docs/design-time-builds.md
                DesignTime = true,
                EnvironmentVariables = { { EnvVar, "RUNNING" } }
            };
            var results = analyzer.Build(environmentOptions);
            // Returns one result per framework target
            var result = results.First();
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
