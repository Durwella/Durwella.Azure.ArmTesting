using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using System.IO;
using Xunit;
using static Durwella.Azure.ArmTesting.Tests.Testing;
using static System.IO.File;
using static System.IO.Path;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class ArmTemplateEnumerationTest
    {
        [Theory(DisplayName = "Empty csproj"), AutoMoqData]
        public void EmptyProject(ArmTemplateEnumeration subject)
        {
            var projectPath = Combine(GetTemporaryDirectory(), "Test.csproj");
            WriteAllText(projectPath, @"<Project Sdk=""Microsoft.NET.Sdk""></Project>");

            subject.EnumerateArmTemplatePaths(projectPath)
                .Should().BeEmpty();
        }

        [Theory(DisplayName = "1 AzureDeploy.json"), AutoMoqData]
        public void OneAzureDeployJson(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var projectPath = Combine(directory, "Test.csproj");
            WriteAllText(projectPath, @"<Project Sdk=""Microsoft.NET.Sdk""></Project>");
            WriteAllText(Combine(directory, "main.cs"), "using System;");
            WriteAllText(Combine(directory, "other.json"), "{}");
            var azureDeployPath = Combine(directory, "azuredeploy.json");
            WriteAllText(azureDeployPath, "{}");

            subject.EnumerateArmTemplatePaths(projectPath)
                .Should().Equal(azureDeployPath);
        }

        [Theory(DisplayName = "2 AzureDeploy.json"), AutoMoqData]
        public void TwoAzureDeployJsonInSubdirectories(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var projectPath = Combine(directory, "Test.csproj");
            WriteAllText(projectPath, @"<Project Sdk=""Microsoft.NET.Sdk""></Project>");
            var subDir1 = Combine(directory, "sub1");
            Directory.CreateDirectory(subDir1);
            var azureDeploy1 = Combine(subDir1, "AzureDeploy.json");
            WriteAllText(azureDeploy1, "{}");
            var subDir2 = Combine(directory, "sub2");
            Directory.CreateDirectory(subDir2);
            var azureDeploy2 = Combine(subDir2, "AzureDeploy.json");
            WriteAllText(azureDeploy2, "{}");

            subject.EnumerateArmTemplatePaths(projectPath)
                .Should().Equal(azureDeploy1, azureDeploy2);
        }

        [Theory(DisplayName = "other.json"), AutoMoqData]
        public void FindJsonFileWithSchema(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var projectPath = Combine(directory, "Test.csproj");
            WriteAllText(projectPath, @"<Project Sdk=""Microsoft.NET.Sdk""></Project>");
            WriteAllText(Combine(directory, "main.cs"), "using System;");
            var armTemplatePath = Combine(directory, "other.json");
            Copy(@"Examples\basic.json", armTemplatePath);

            subject.EnumerateArmTemplatePaths(projectPath)
                .Should().Equal(armTemplatePath);
        }

        [Theory(DisplayName = "/sub/other.json"), AutoMoqData]
        public void FindJsonFileInSubdirectoryWithSchema(ArmTemplateEnumeration subject)
        {
            var directory = GetTemporaryDirectory();
            var projectPath = Combine(directory, "Test.csproj");
            WriteAllText(projectPath, @"<Project Sdk=""Microsoft.NET.Sdk""></Project>");
            WriteAllText(Combine(directory, "project.json"), "{ }");
            var subDir = Combine(directory, "sub");
            Directory.CreateDirectory(subDir);
            var armTemplatePath = Combine(subDir, "other.json");
            Copy(@"Examples\basic.json", armTemplatePath);

            subject.EnumerateArmTemplatePaths(projectPath)
                .Should().Equal(armTemplatePath);
        }

        // TODO: Exclude bin, obj, .vs folders

    }
}
