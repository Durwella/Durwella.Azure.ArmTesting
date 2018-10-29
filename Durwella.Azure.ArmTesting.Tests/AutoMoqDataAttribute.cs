using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Durwella.Azure.ArmTesting.Tests
{
    internal class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute(bool configureMembers = false)
            : base(() => BuildFixture(configureMembers))
        {
        }

        private static IFixture BuildFixture(bool configureMembers)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = configureMembers });
            return fixture;
        }
    }
}