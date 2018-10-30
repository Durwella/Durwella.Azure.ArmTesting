using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.Services
{
    public class NameChecking
    {
        private const int StorageMinNameLength = 3;
        private const int StorageMaxNameLength = 24;
        private const string StorageAccountType = "Microsoft.Storage/storageAccounts";

        private string StorageNamingRule => $"Storage account name must be between {StorageMinNameLength} and {StorageMaxNameLength} characters in length.";

        public IEnumerable<TokenError> CheckTemplate(string armTemplateJson)
        {
            var loadSettings = new JsonLoadSettings
            {
                LineInfoHandling = LineInfoHandling.Load
            };
            var root = JObject.Parse(armTemplateJson, loadSettings);
            return CheckTemplate(root);
        }

        public IEnumerable<TokenError> CheckTemplate(JObject root)
        {
            var resources = root["resources"];
            foreach (var resource in resources)
            {
                // Ignore non-Storage resources
                var typeToken = resource["type"];
                if (typeToken.Value<string>() != StorageAccountType)
                    continue;
                var nameToken = resource["name"];
                var name = nameToken.Value<string>();
                // Ignore names generated from a function call
                if (name.StartsWith("[") && name.EndsWith("]"))
                    continue;
                string adjective = null;
                if (name.Length < StorageMinNameLength)
                    adjective = "short";
                else if (name.Length > StorageMaxNameLength)
                    adjective = "long";
                if (adjective != null)
                    yield return new TokenError(
                        nameToken, $"The name '{name}' is too {adjective} (at {name.Length} characters). {StorageNamingRule}");
            }
        }
    }
}
