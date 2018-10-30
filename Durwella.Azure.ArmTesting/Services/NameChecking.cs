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
                var name = resource["name"];
                var lineInfo = (IJsonLineInfo)name;
                yield return new ArmTemplateError(null, lineInfo.LineNumber, "The name is too long");
            }


            //return new List<ArmTemplateError>();
            // 18:17:51 -     : zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz is not a valid storage account name. Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only.

        }
    }
}
