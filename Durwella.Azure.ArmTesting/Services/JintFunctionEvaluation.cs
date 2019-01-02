using Jint;
using Jint.Native;
using Jint.Runtime;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.Services
{
    public class JintFunctionEvaluation
    {
        public string Evaluate(string armFunctionExpression, Dictionary<string, string> variables = null)
        {
            var engine = new Engine();
            // Set up the variables function to look up into given dictionary
            variables = variables ?? new Dictionary<string, string>();
            engine.SetValue("_variables", variables);
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
            //var dict = new Dictionary<string, string>();
            //foreach (var variable in variablesObject)
            //{
            //    var key = variable.Key;
            //    var value = variable.Value.Value<string>();
            //    dict.Add(key, value);
            //}

            var expression = root["resources"][0]["name"].ToString();
            var result = Evaluate(expression, variablesDictionary);
            var output = armTemplate.Replace('"' + expression + '"', result);
            return output;
        }
    }
}
