using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using Xunit;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class ArmTemplateEnumerationTest
    {
        [Theory(DisplayName = "Empty Dir"), AutoMoqData]
        public void EmptyDirectory(ArmTemplateEnumeration subject)
        {
            var directory = Testing.GetTemporaryDirectory();

            subject.EnumerateArmTemplatePaths(directory)
                .Should().BeEmpty();
        }

        [Theory(DisplayName = "Empty Dir"), AutoMoqData]
        public void FlatDirectory(ArmTemplateEnumeration subject)
        {
            var directory = Testing.GetTemporaryDirectory();

            subject.EnumerateArmTemplatePaths(directory)
                .Should().BeEmpty();
        }
    }
}
