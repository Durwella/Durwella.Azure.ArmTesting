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
            var name = "verylongstoragenamelongerthan24chars";
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", name);

            var errors = subject.CheckResourceNames(json);

            errors.Should().HaveCount(1);
            var error = errors.Single();
            error.LineNumber.Should().Be(8);
            error.Message.Should().Contain($"'{name}' is too long");
        }

        [Theory(DisplayName = "Storage: short name"), AutoMoqData]
        public void StorageNameTooShort(NameChecking subject)
        {
            var name = "a1";
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", name);

            var errors = subject.CheckResourceNames(json);

            errors.Should().HaveCount(1);
            var error = errors.Single();
            error.LineNumber.Should().Be(8);
            error.Message.Should().Contain($"'{name}' is too short");
        }

        [Theory(DisplayName = "Storage: ok"), AutoMoqData]
        public void StorageNameOk(NameChecking subject)
        {
            var name = "abc";
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", name);

            var errors = subject.CheckResourceNames(json);

            errors.Should().BeEmpty();
        }
    }
}
