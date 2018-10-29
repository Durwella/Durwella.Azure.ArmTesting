using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
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
        private static IEnumerable<string> EnumerateArmTemplatePaths(ArmTemplateEnumeration subject, string directory)
        {
            var projectPath = CreateProjectFile(directory);
            // Act
            return subject.EnumerateArmTemplatePaths(projectPath);
        }

        [Theory(DisplayName = "Empty csproj"), AutoMoqData]
        public void EmptyProject(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();

            var armTemplatePaths = EnumerateArmTemplatePaths(subject, directory);

            armTemplatePaths.Should().BeEmpty();
        }

        [Theory(DisplayName = "1 AzureDeploy.json"), AutoMoqData]
        public void OneAzureDeployJson(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            WriteAllText(Combine(directory, "main.cs"), "using System;");
            WriteAllText(Combine(directory, "other.json"), "{}");
            var azureDeployPath = Combine(directory, "azuredeploy.json");
            WriteAllText(azureDeployPath, "{}");

            var armTemplatePaths = EnumerateArmTemplatePaths(subject, directory);

            armTemplatePaths.Should().Equal(azureDeployPath);
        }

        [Theory(DisplayName = "2 AzureDeploy.json"), AutoMoqData]
        public void TwoAzureDeployJsonInSubdirectories(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var subDir1 = CreateDirectory(directory, "sub1");
            var azureDeploy1 = Combine(subDir1, "AzureDeploy.json");
            WriteAllText(azureDeploy1, "{}");
            var subDir2 = CreateDirectory(directory, "sub2");
            var azureDeploy2 = Combine(subDir2, "AzureDeploy.json");
            WriteAllText(azureDeploy2, "{}");

            var armTemplatePaths = EnumerateArmTemplatePaths(subject, directory);

            armTemplatePaths.Should().Equal(azureDeploy1, azureDeploy2);
        }

        [Theory(DisplayName = "other.json"), AutoMoqData]
        public void FindJsonFileWithSchema(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            WriteAllText(Combine(directory, "main.cs"), "using System;");
            var armTemplatePath = Combine(directory, "other.json");
            Copy(@"Examples\basic.json", armTemplatePath);

            var armTemplatePaths = EnumerateArmTemplatePaths(subject, directory);

            armTemplatePaths.Should().Equal(armTemplatePath);
        }

        [Theory(DisplayName = "/sub/other.json"), AutoMoqData]
        public void FindJsonFileInSubdirectoryWithSchema(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            WriteAllText(Combine(directory, "project.json"), "{ }");
            var subDir = CreateDirectory(directory, "sub");
            var armTemplatePath = Combine(subDir, "other.json");
            Copy(@"Examples\basic.json", armTemplatePath);

            var armTemplatePaths = EnumerateArmTemplatePaths(subject, directory);

            armTemplatePaths.Should().Equal(armTemplatePath);
        }

        [Theory(DisplayName = "Not /bin/...")]
        [InlineAutoMoqData("bin")]
        [InlineAutoMoqData("obj")]
        [InlineAutoMoqData(".vs")]
        public void IgnoreBinDirectory(string binDir, ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var bin = CreateDirectory(directory, binDir);
            Copy(@"Examples\basic.json", Combine(bin, "azuredeploy.json"));
            var sub = CreateDirectory(bin, "sub");
            Copy(@"Examples\basic.json", Combine(sub, "azuredeploy.json"));

            var armTemplatePaths = EnumerateArmTemplatePaths(subject, directory);

            armTemplatePaths.Should().BeEmpty();
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
