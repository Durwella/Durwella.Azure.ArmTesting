using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using Xunit;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class JintFunctionEvaluationTest
    {
        [Theory(DisplayName = "array()")]
        [InlineAutoMoqData("1", "[1]")]
        [InlineAutoMoqData("'efgh'", "['efgh']")]
        [InlineAutoMoqData("{a: 'b', c: 'd'}", "[{'a':'b','c':'d'}]")]
        public void ArrayFunction(string argument, string expected, JintFunctionEvaluation subject)
        {
            var expression = $"[array({argument})]";

            var result = subject.Evaluate(expression);

            result.Should().Be(expected);
        }

        [Theory(DisplayName = "concat()")]
        [InlineAutoMoqData("'a', 'b', 'c'", "'abc'")]
        [InlineAutoMoqData("'el', 'ep', 'ha', 'nt'", "'elephant'")]
        [InlineAutoMoqData("'vm', 2", "'vm2'")]
        public void ConcatFunction(string concatArgs, string expected, JintFunctionEvaluation subject)
        {
            var expression = $"[concat({concatArgs})]";

            var result = subject.Evaluate(expression);

            result.Should().Be(expected);
        }
    }
}
