using AutoFixture.Xunit2;
using Xunit;
using Xunit.Sdk;

namespace Durwella.Azure.ArmTesting.Tests
{
    /// <summary>
    /// Allows use of Inline data for some parameters with Moq generation of remaining parameters.
    /// Declare inline data parameters first.
    /// </summary>
    internal class InlineAutoMoqDataAttribute : CompositeDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] values)
            : base(new DataAttribute[] {
            new InlineDataAttribute(values), new AutoMoqDataAttribute() })
        {
        }
    }
}