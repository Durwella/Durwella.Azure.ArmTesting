using Jint;

namespace Durwella.Azure.ArmTesting.Services
{
    public class JintFunctionEvaluation
    {
        public object Evaluate(string armFunctionExpression)
        {
            var engine = new Engine();
            engine.Execute(@"
                function array(arg) {
                    return [arg];
                }

                function concat() {
                  return ''.concat.apply('', arguments);
                }
            ");
            var expression = armFunctionExpression.TrimStart('[').TrimEnd(']');
            return engine.Execute(expression)
                .GetCompletionValue()
                .AsString();
        }
    }
}
