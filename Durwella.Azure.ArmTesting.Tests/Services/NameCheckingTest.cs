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
            var errors = subject.CheckTemplate(json);
            errors.Should().BeEmpty();
        }

        // Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only.

        [Theory(DisplayName = "Storage: long name"), AutoMoqData]
        public void StorageNameTooLong(NameChecking subject)
        {
            var name = "verylongstoragenamelongerthan24chars";
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", name);

            var errors = subject.CheckTemplate(json);

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

            var errors = subject.CheckTemplate(json);

            errors.Should().HaveCount(1);
            var error = errors.Single();
            error.LineNumber.Should().Be(8);
            error.Message.Should().Contain($"'{name}' is too short");
        }

        [Theory(DisplayName = "Storage: ok")]
        [InlineAutoMoqData("abc")]
        [InlineAutoMoqData("a1234567890")]
        [InlineAutoMoqData("a12345678901234567890bc")]
        [InlineAutoMoqData("[concat(uniquestring(resourceGroup().id), 'standardsa')]")] // Ignoring function calls currently
        public void StorageNameOk(string name, NameChecking subject)
        {
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", name);

            var errors = subject.CheckTemplate(json);

            errors.Should().BeEmpty();
        }

        [Theory(DisplayName = "Other type: ok"), AutoMoqData]
        public void IgnoreLongNameUnknownResourceType(NameChecking subject)
        {
            var json = ReadAllText(Combine("Examples", "storage-basic.json"))
                .Replace("storagename", "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz")
                .Replace("Microsoft.Storage/storageAccounts", "Durwella.Cloud/Unknown");

            var errors = subject.CheckTemplate(json);

            errors.Should().BeEmpty();
        }
    }
}
