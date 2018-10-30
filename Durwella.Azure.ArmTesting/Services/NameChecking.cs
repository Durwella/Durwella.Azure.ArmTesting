using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.Services
{
    public class NameChecking
    {
        public IEnumerable<ArmTemplateError> CheckResourceNames(string armTemplateJson)
        {
            var loadSettings = new JsonLoadSettings
            {
                LineInfoHandling = LineInfoHandling.Load
            };
            var root = JObject.Parse(armTemplateJson, loadSettings);
            var resources = root["resources"];
            foreach (var resource in resources)
            {
                var nameToken = resource["name"];
                var lineInfo = (IJsonLineInfo)nameToken;
                var name = nameToken.Value<string>();
                if (name.Length < 3)                
                    yield return new ArmTemplateError(
                        lineInfo,
                        $"The name '{name}' is too short. " +
                        "Storage account name must be between 3 and 24 characters in length.");
                else if (name.Length > 24)
                    yield return new ArmTemplateError(
                        lineInfo,
                        $"The name '{name}' is too long. " +
                        "Storage account name must be between 3 and 24 characters in length.");
            }


            //return new List<ArmTemplateError>();
            // 18:17:51 -     : zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz is not a valid storage account name. Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only.

        }
    }
}
