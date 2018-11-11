using Jint;
using Jint.Native;
using Jint.Runtime;

namespace Durwella.Azure.ArmTesting.Services
{
    public class JintFunctionEvaluation
    {
        public string Evaluate(string armFunctionExpression)
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
            // The JINT REPL example was helpful for this code
            // https://github.com/sebastienros/jint/blob/388854cbf81274eb01239f2c7b23c7799d0d4f79/Jint.Repl/Program.cs#L52
            var result = engine.Execute(expression)
                .GetCompletionValue();
            if (result.Type != Types.None && result.Type != Types.Null && result.Type != Types.Undefined)
            {
                // Call JSON.stringify
                JsValue json = engine.Json.Stringify(engine.Json, Arguments.From(result));
                // Convert to a string
                var str = TypeConverter.ToString(json);
                // HACK: Use single-quotes for strings (this is too dumb)
                str = str.Replace('\"', '\'');
                return str;
            }
            // TODO: Handle null, undefined, exceptions...
            return "null";
        }
    }
}
