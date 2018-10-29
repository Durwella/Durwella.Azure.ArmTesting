using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Durwella.Azure.ArmTesting.Services
{
    public class ArmTemplateEnumeration
    {
        /// <summary>
        /// The conventional name for ARM templates is azuredeploy.json.
        /// Templates are not required to have this name, but is commonly used
        /// as in https://github.com/Azure/azure-quickstart-templates
        /// and by http://azuredeploy.net
        /// </summary>
        const string AzureDeployJson = "azuredeploy.json";

        /// <summary>
        /// URL of the JSON schema file that describes the version of the ARM template language.
        /// https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates#template-format
        /// </summary>
        const string TemplateJsonSchema = "http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json";

        /// <summary>
        /// Subdirectories of the project directory that we should ignore when looking for ARM templates
        /// </summary>
        private static readonly IList<string> ExcludedDirectories = new[] { "bin", "obj", ".vs" };

        /// <summary>
        /// Searches Project directory recursively for all *.json files that smell like ARM templates
        /// </summary>
        public IEnumerable<string> EnumerateArmTemplatePaths(string projectPath)
        {
            var directory = Path.GetDirectoryName(projectPath);
            return EnumerateArmTemplatePathsFromDirectory(directory);
        }

        /// <summary>
        /// Searches directory recursively for all *.json files that smell like ARM templates
        /// </summary>
        private static IEnumerable<string> EnumerateArmTemplatePathsFromDirectory(string directory)
        {
            var jsonFiles = Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories);
            foreach (var path in jsonFiles)
            {
                // Exclude files that are in directories like ./bin
                if (IsInExcludedDirectory(directory, path))
                    continue;
                // If it is named according to convention, assume it is an ARM template
                if (Path.GetFileName(path).Equals(AzureDeployJson, StringComparison.OrdinalIgnoreCase))
                    yield return path;
                // HACK: Peeking at first two lines to check for ARM schema URL
                using (var reader = new StreamReader(File.OpenRead(path)))
                {
                    var text = reader.ReadLine() + reader.ReadLine();
                    if (text.Contains(TemplateJsonSchema))
                        yield return path;
                }
            }
        }

        private static bool IsInExcludedDirectory(string projectDirectory, string path)
        {
            var relativePath = GetRelativePath(projectDirectory, path);
            // The relative path will start with ".\" so we look at the second segment of the path:
            var subdirectoryName = relativePath.Split(Path.DirectorySeparatorChar)[1];
            return ExcludedDirectories.Contains(subdirectoryName);
        }

        #region Relative Path

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// Same signature as Path.GetRelativePath
        /// which is unavailable for the NetStandard target of this project.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getrelativepath?view=netcore-2.1
        /// </summary>
        static string GetRelativePath(string relativeTo, string path)
        {
            // Inspired by @ctacke https://stackoverflow.com/a/485516/483776
            const int MaxPath = 260;
            var relativePath = new StringBuilder(MaxPath);
            if (!PathRelativePathTo(relativePath,
                relativeTo, File.GetAttributes(relativeTo),
                path, File.GetAttributes(path)))
            {
                throw new ArgumentException("PathRelativePathTo: Paths must have a common prefix");
            }
            return relativePath.ToString();
        }

        /// <summary>
        ///  Creates a relative path from one file or folder to another.
        ///  http://pinvoke.net/default.aspx/shlwapi.PathRelativePathTo
        ///  https://docs.microsoft.com/en-us/windows/desktop/api/shlwapi/nf-shlwapi-pathrelativepathtoa
        /// </summary>
        /// <param name="pszPath">A pointer to a string that receives the relative path. This buffer must be at least MAX_PATH characters in size.</param>
        /// <param name="pszFrom">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path that defines the start of the relative path.</param>
        /// <param name="dwAttrFrom">The file attributes of pszFrom. If this value contains FILE_ATTRIBUTE_DIRECTORY, pszFrom is assumed to be a directory; otherwise, pszFrom is assumed to be a file.</param>
        /// <param name="pszTo">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path that defines the endpoint of the relative path.</param>
        /// <param name="dwAttrTo">The file attributes of pszTo. If this value contains FILE_ATTRIBUTE_DIRECTORY, pszTo is assumed to be directory; otherwise, pszTo is assumed to be a file.</param>
        /// <returns>Returns TRUE if successful, or FALSE otherwise.</returns>
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        static extern bool PathRelativePathTo(
             [Out] StringBuilder pszPath,
             [In] string pszFrom,
             [In] FileAttributes dwAttrFrom,
             [In] string pszTo,
             [In] FileAttributes dwAttrTo
        );

        #endregion
    }
}
