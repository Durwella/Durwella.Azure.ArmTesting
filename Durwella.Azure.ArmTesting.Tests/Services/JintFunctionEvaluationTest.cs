using Durwella.Azure.ArmTesting.Services;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Durwella.Azure.ArmTesting.Tests.Services
{
    public class JintFunctionEvaluationTest
    {
        // https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions-array#array
        [Theory(DisplayName = "array()")]
        [InlineAutoMoqData("1", "[1]")]
        [InlineAutoMoqData("'efgh'", "[\"efgh\"]")]
        [InlineAutoMoqData("{a: 'b', c: 'd'}", "[{\"a\":\"b\",\"c\":\"d\"}]")]
        public void ArrayFunction(string argument, string expected, JintFunctionEvaluation subject)
        {
            var expression = $"[array({argument})]";

            var result = subject.Evaluate(expression);

            result.Should().Be(expected);
        }

        // https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions-array#concat
        [Theory(DisplayName = "concat()")]
        [InlineAutoMoqData("'a', 'b', 'c'", "\"abc\"")]
        [InlineAutoMoqData("'el', 'ep', 'ha', 'nt'", "\"elephant\"")]
        [InlineAutoMoqData("'vm', 2", "\"vm2\"")]
        public void ConcatFunction(string concatArgs, string expected, JintFunctionEvaluation subject)
        {
            var expression = $"[concat({concatArgs})]";

            var result = subject.Evaluate(expression);

            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Integration"), AutoMoqData]
        public void ConcatIntegration(JintFunctionEvaluation subject, NameChecking nameChecking)
        {
            var path = Path.Combine("Examples", "storage-concat-name.json");
            var text = File.ReadAllText(path);

            var output = subject.EvaluateAndReplaceFunctions(text);

            output.Should().Contain("\"name\": \"abc\",");
            nameChecking.CheckTemplate(output).Should().BeEmpty();
        }

        [Theory(DisplayName = "variable name"), AutoMoqData]
        public void VariableFunctionIntegration(JintFunctionEvaluation subject, NameChecking nameChecking)
        {
            var path = Path.Combine("Examples", "storage-variable-name.json");
            var text = File.ReadAllText(path);

            var output = subject.EvaluateAndReplaceFunctions(text);

            output.Should().Contain("\"name\": \"my_storage\",");
            nameChecking.CheckTemplate(output).Should().BeEmpty();
        }

        // TODO: Don't evaluate `[[...]]`, but convert to `[`
    }
}
