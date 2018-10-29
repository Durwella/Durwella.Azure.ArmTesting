using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class ArmTemplateEnumerationTest
    {
        [Theory(DisplayName = "Empty Proj"), AutoMoqData]
        public void EmptyProject(ArmTemplateEnumeration subject)
        {
            var projectPath = Path.Combine(Testing.GetTemporaryDirectory(), "Test.csproj");
            File.WriteAllText(projectPath, @"<Project></Project>");

            subject.EnumerateArmTemplatePaths(projectPath)
                .Should().BeEmpty();
        }
    }
}
