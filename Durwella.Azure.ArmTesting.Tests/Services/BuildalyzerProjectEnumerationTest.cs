using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using System.IO;
using Xunit;
using static Durwella.Azure.ArmTesting.Tests.Testing;
using static System.IO.File;
using static System.IO.Path;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class BuildalyzerProjectEnumerationTest
    {
        [Theory(DisplayName = "Empty csproj"), AutoMoqData]
        public void EmptyProject(BuildalyzerProjectEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var projectPath = CreateProjectFile(directory);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().BeEmpty();
        }

        [Theory(DisplayName = "main.cs"), AutoMoqData]
        public void OneMainCs(BuildalyzerProjectEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var filePath = Create(directory, "main.cs");
            var projectPath = CreateProjectFile(directory);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().Equal(filePath);
        }

        [Theory(DisplayName = "sample.json"), AutoMoqData]
        public void OneSampleJson(BuildalyzerProjectEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var filePath = Create(directory, "sample.json");
            var projectPath = CreateProjectFile(directory);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().Equal(filePath);
        }

        [Theory(DisplayName = "multiple in /"), AutoMoqData]
        public void MultipleFilesInDirectory(BuildalyzerProjectEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            string filePath1 = Create(directory, "file1.txt");
            string filePath2 = Create(directory, "file2.cs");
            string filePath3 = Create(directory, "file3.json");
            var projectPath = CreateProjectFile(directory);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().BeEquivalentTo(filePath1, filePath2, filePath3);
        }

        [Theory(DisplayName = "multiple in /sub1 /sub2"), AutoMoqData]
        public void MultipleFilesInSubdirectories(BuildalyzerProjectEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            string filePath1 = Create(directory, "sub1", "file1.json");
            string filePath2 = Create(directory, "sub2", "file2.cs");
            var projectPath = CreateProjectFile(directory);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().BeEquivalentTo(filePath1, filePath2);
        }

        [Theory(DisplayName = "Not /bin/...")]
        [InlineAutoMoqData("bin")]
        [InlineAutoMoqData("obj")]
        [InlineAutoMoqData(".vs")]
        public void IgnoreBinDirectory(string binDir, BuildalyzerProjectEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            Create(directory, binDir, "file.json");
            var projectPath = CreateProjectFile(directory);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().BeEmpty();
        }

        [Theory(DisplayName = "../embedded.json"), AutoMoqData]
        public void EmbeddedAndLinkedFile(BuildalyzerProjectEnumeration subject)
        {
            var otherDirectory = GetTemporaryDirectory();
            var otherDirName = Path.GetFileName(otherDirectory);
            var filePath = Create(otherDirectory, "sample.json");
            var directory = GetTemporaryDirectory();
            var link = $"<EmbeddedResource " +
                $" Include=\"..\\{otherDirName}\\sample.json\" " +
                $" Link=\"FakeDirectory\\sample.json\" />";
            var projectPath = CreateProjectFile(directory, link);

            var filePaths = subject.EnumerateProjectFiles(projectPath);

            filePaths.Should().Equal(filePath);
        }

        private static string Create(string directory, string name)
        {
            var filePath = Combine(directory, name);
            CreateText(filePath);
            return filePath;
        }

        private static string Create(string directory, string subdirectory, string name)
        {
            Directory.CreateDirectory(Combine(directory, subdirectory));
            var filePath = Combine(directory, subdirectory, name);
            CreateText(filePath);
            return filePath;
        }

        private static string CreateProjectFile(string directory, string content = "")
        {
            var projectPath = Combine(directory, "Test.csproj");
            WriteAllText(projectPath,
                $"<Project Sdk=\"Microsoft.NET.Sdk\">" +
                $"  <ItemGroup>{content}</ItemGroup>" +
                $"</Project>");
            return projectPath;
        }
    }
}
