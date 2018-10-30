using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using System.Linq;
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

        // Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only.

        [Theory(DisplayName = "Storage: long name"), AutoMoqData]
        public void StorageNameTooLong(NameChecking subject)
        {
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", "verylongstoragenamelongerthan24chars");

            var errors = subject.CheckResourceNames(json);

            errors.Should().HaveCount(1);
            var error = errors.Single();
            error.LineNumber.Should().Be(8);
            error.Message.Should().Contain("too long");
        }
    }
}
