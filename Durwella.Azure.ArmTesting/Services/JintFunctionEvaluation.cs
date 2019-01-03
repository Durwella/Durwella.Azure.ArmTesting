using Jint;
using Jint.Native;
using Jint.Runtime;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Durwella.Azure.ArmTesting.Services
{
    public class JintFunctionEvaluation : IArmFunctionEvaluation
    {
        public string Evaluate(string armFunctionExpression, Dictionary<string, string> variables = null)
        {
            if (!armFunctionExpression.StartsWith("[") || !armFunctionExpression.EndsWith("]"))
                return armFunctionExpression;
            // Evaluate variables first as they may include expressions (and reference prior variables)
            variables = variables ?? new Dictionary<string, string>();
            var variablesEvaluated = new Dictionary<string, string>();
            foreach (var variable in variables)
            {
                var value = Evaluate(variable.Value, variablesEvaluated);
                // HACK: Remove quotes wrapping string result in this case... Somewhat inconsistent w/ the general case.
                value = value.TrimStart('"').TrimEnd('"');
                variablesEvaluated.Add(variable.Key, value);
            }
            var engine = new Engine();
            // Set up the variables function to look up into given dictionary
            engine.SetValue("_variables", variablesEvaluated);
            engine.Execute(@"
                function variables(name) {
                    return _variables[name];
                }

                function array(arg) {
                    return [arg];
                }

                function concat() {
                  return ''.concat.apply('', arguments);
                }

                function resourceGroup() {
                    return { location: '' };
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
                return TypeConverter.ToString(json);
            }
            // TODO: Handle null, undefined, exceptions...
            return "null";
        }

        public string EvaluateAndReplaceFunctions(string armTemplate)
        {
            var root = JObject.Parse(armTemplate);
            var variablesObject = (JObject)root["variables"];
            var variablesDictionary = variablesObject.ToObject<Dictionary<string, string>>();
            // TODO: evaluate variables only once
            var leaves = root.Descendants().Where(p => !p.HasValues);
            foreach (var leaf in leaves)
            {
                // TODO: Test if is string
                var expression = leaf.ToString();
                if (expression.StartsWith("[") && expression.EndsWith("]"))
                {
                    var result = Evaluate(expression, variablesDictionary);
                    // TODO: Format any error in evaluation of JS function
                    armTemplate = armTemplate.Replace('"' + expression + '"', result);
                }
            }
            return armTemplate;
        }
    }
}
