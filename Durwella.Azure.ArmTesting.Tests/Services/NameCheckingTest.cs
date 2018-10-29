using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using Xunit;
using static System.IO.File;
using static System.IO.Path;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class NameCheckingTest
    {
        [Theory(DisplayName = "Basic"), AutoMoqData]
        public void BasicTemplateNoErrors(NameChecking subject)
        {
            var json = ReadAllText(Combine("Examples", "basic.json"));
            var errors = subject.CheckResourceNames(json);
            errors.Should().BeEmpty();
        }
    }
}
