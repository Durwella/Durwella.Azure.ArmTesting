using Jint;

namespace Durwella.Azure.ArmTesting.Services
{
    public class JintFunctionEvaluation
    {
        public string Evaluate(string armFunctionExpression)
        {
            var engine = new Engine();
            engine.Execute(@"
                function concat() {
                  return ''.concat.apply('', arguments);
                }
            ");
            var expression = armFunctionExpression.TrimStart('[').TrimEnd(']');
            return engine.Execute(expression)
                .GetCompletionValue().AsString();
        }
    }
}
