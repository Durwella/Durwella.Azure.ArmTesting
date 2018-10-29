using System.IO;

namespace Durwella.Azure.ArmTesting.Tests
{
    class Testing
    {
        public static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}