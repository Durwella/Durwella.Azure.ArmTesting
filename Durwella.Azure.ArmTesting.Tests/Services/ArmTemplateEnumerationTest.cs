using AutoFixture.Xunit2;
using Durwella.Azure.ArmTesting.ServiceInterfaces;
using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static Durwella.Azure.ArmTesting.Tests.Testing;
using static System.IO.File;
using static System.IO.Path;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class ArmTemplateEnumerationTest
    {
        private static string BasicJsonPath => Combine("Examples", "basic.json");

        [Theory(DisplayName = "Empty csproj"), AutoMoqData]
        public void EmptyProject(
            string projectPath,
            [Frozen]Mock<IProjectFileEnumeration> projectFileEnumeration,
            ArmTemplateEnumeration subject)
        {
            projectFileEnumeration.Setup(x => x.EnumerateProjectFiles(projectPath))
                .Returns(new List<string>());

            var armTemplatePaths = subject.EnumerateArmTemplatePaths(projectPath);

            armTemplatePaths.Should().BeEmpty();
        }

        [Theory(DisplayName = "1 AzureDeploy.json"), AutoMoqData]
        public void OneAzureDeployJson(
            string projectPath,
            [Frozen]Mock<IProjectFileEnumeration> projectFileEnumeration,
            ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var main = Write(directory, "main.cs", "using System;");
            var other = Write(directory, "other.json", "{}");
            var azureDeployPath = Write(directory, "azuredeploy.json", "{}");
            projectFileEnumeration.Setup(x => x.EnumerateProjectFiles(projectPath))
                .Returns(new[] { main, other, azureDeployPath });

            var armTemplatePaths = subject.EnumerateArmTemplatePaths(projectPath);

            armTemplatePaths.Should().Equal(azureDeployPath);
        }

        [Theory(DisplayName = "1 AzureDeploy.json w/ schema"), AutoMoqData]
        public void OneAzureDeployJsonWithBasicSchema(
            string projectPath,
            [Frozen]Mock<IProjectFileEnumeration> projectFileEnumeration,
            ArmTemplateEnumeration subject)
        {
            // This makes sure we don't double-count azuredeploy.json files w/ the expected content
            var directory = GetTemporaryDirectory();
            var other = Write(directory, "other.json", "{}");
            var azureDeployPath = Combine(directory, "azuredeploy.json");
            Copy(BasicJsonPath, azureDeployPath);
            projectFileEnumeration.Setup(x => x.EnumerateProjectFiles(projectPath))
                .Returns(new[] { other, azureDeployPath });

            var armTemplatePaths = subject.EnumerateArmTemplatePaths(projectPath);

            armTemplatePaths.Should().Equal(azureDeployPath);
        }

        [Theory(DisplayName = "2 AzureDeploy.json"), AutoMoqData]
        public void TwoAzureDeployJsonInSubdirectories(
            string projectPath,
            [Frozen]Mock<IProjectFileEnumeration> projectFileEnumeration,
            ArmTemplateEnumeration subject)
        {
            // Ensure case is ignored and subdirectories used
            var directory = GetTemporaryDirectory();
            var azureDeploy1 = Write(directory, "sub1", "AzureDeploy.json", "{}");
            var azureDeploy2 = Write(directory, "sub1", "AZUREDEPLOY.JSON", "{}");
            projectFileEnumeration.Setup(x => x.EnumerateProjectFiles(projectPath))
                .Returns(new[] { azureDeploy1, azureDeploy2 });

            var armTemplatePaths = subject.EnumerateArmTemplatePaths(projectPath);

            armTemplatePaths.Should().Equal(azureDeploy1, azureDeploy2);
        }

        [Theory(DisplayName = "other.json"), AutoMoqData]
        public void FindJsonFileWithSchema(
            string projectPath,
            [Frozen]Mock<IProjectFileEnumeration> projectFileEnumeration,
            ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var main = Write(directory, "main.cs", "using System;");
            var armTemplatePath = Combine(directory, "other.json");
            Copy(BasicJsonPath, armTemplatePath);
            projectFileEnumeration.Setup(x => x.EnumerateProjectFiles(projectPath))
                .Returns(new[] { main, armTemplatePath });

            var armTemplatePaths = subject.EnumerateArmTemplatePaths(projectPath);

            armTemplatePaths.Should().Equal(armTemplatePath);
        }

        [Theory(DisplayName = "https schema"), AutoMoqData]
        public void FindJsonFileWithHttpsSchema(
            string projectPath,
            [Frozen]Mock<IProjectFileEnumeration> projectFileEnumeration,
            ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var main = Write(directory, "main.cs", "using System;");
            var json = ReadAllText(BasicJsonPath)
                .Replace("http://", "https://");
            var armTemplatePath = Write(directory, "other.json", json);
            projectFileEnumeration.Setup(x => x.EnumerateProjectFiles(projectPath))
                .Returns(new[] { main, armTemplatePath });

            var armTemplatePaths = subject.EnumerateArmTemplatePaths(projectPath);

            armTemplatePaths.Should().Equal(armTemplatePath);
        }

        private static string Write(string directory, string name, string content)
        {
            var filePath = Combine(directory, name);
            WriteAllText(filePath, content);
            return filePath;
        }

        private static string Write(string directory, string subdirectory, string name, string content)
        {
            directory = CreateDirectory(directory, subdirectory);
            var filePath = Combine(directory, name);
            WriteAllText(filePath, content);
            return filePath;
        }

        private static string CreateProjectFile(string directory)
        {
            var projectPath = Combine(directory, "Test.csproj");
            WriteAllText(projectPath, @"<Project Sdk=""Microsoft.NET.Sdk""></Project>");
            return projectPath;
        }

        private static string CreateDirectory(string directory, string subDirectoryName)
        {
            var subdirectoryPath = Combine(directory, subDirectoryName);
            Directory.CreateDirectory(subdirectoryPath);
            return subdirectoryPath;
        }
    }
}
