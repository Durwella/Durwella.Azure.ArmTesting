using Durwella.Azure.ArmTesting.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Durwella.Azure.ArmTesting.Services
{
    /// <summary>
    /// This implementation of IProjectFileEnumeration ignores the .csproj and just naively searches the project directory
    /// on the file system. This will fail to find files that are linked from other directories, 
    /// and it will incorrectly include files that are excluded from the project.
    /// It's not a good implementation.
    /// </summary>
    public class FileSystemProjectEnumeration : IProjectFileEnumeration
    {
        /// <summary>
        /// Subdirectories of the project directory that we should ignore when looking for ARM templates
        /// </summary>
        private static readonly IList<string> ExcludedDirectories = new[] { "bin", "obj", ".vs" };

        public IEnumerable<string> EnumerateProjectFiles(string projectPath)
        {
            var directory = Path.GetDirectoryName(projectPath);
            var allFiles = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);
            // Exclude files that are in directories like ./bin
            return allFiles.Where(path => !IsInExcludedDirectory(directory, path));
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
