using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using Xunit;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class JintFunctionEvaluationTest
    {
        [Theory(DisplayName = "concat()")]
        [InlineAutoMoqData("'a', 'b', 'c'", "abc")]
        [InlineAutoMoqData("'el', 'ep', 'ha', 'nt'", "elephant")]
        [InlineAutoMoqData("'vm', 2", "vm2")]
        public void ConcatFunction(string concatArgs, string expected, JintFunctionEvaluation subject)
        {
            var expression = $"[concat({concatArgs})]";

            var result = subject.Evaluate(expression);

            result.Should().Be(expected);
        }
    }
}
