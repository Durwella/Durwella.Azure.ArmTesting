using Durwella.Azure.ArmTesting.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Durwella.Azure.ArmTesting.Services
{
    public class ArmTemplateEnumeration : IArmTemplateEnumeration
    {
        /// <summary>
        /// The conventional name for ARM templates is azuredeploy.json.
        /// Templates are not required to have this name, but is commonly used
        /// as in https://github.com/Azure/azure-quickstart-templates
        /// and by http://azuredeploy.net
        /// </summary>
        const string AzureDeployJson = "azuredeploy.json";

        /// <summary>
        /// URL of the JSON schema file that describes the version of the ARM template language.
        /// Could be http or https. Sometimes ends with #.
        /// https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates#template-format
        /// </summary>
        const string TemplateJsonSchema = "//schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json";

        private readonly IProjectFileEnumeration _projectFileEnumeration;

        public ArmTemplateEnumeration(FileSystemProjectEnumeration projectFileEnumeration)
        {
            _projectFileEnumeration = projectFileEnumeration;
        }

        /// <summary>
        /// Searches Project directory recursively for all *.json files that smell like ARM templates
        /// </summary>
        public IEnumerable<string> EnumerateArmTemplatePaths(string projectPath)
        {
            var jsonFiles = _projectFileEnumeration.EnumerateProjectFiles(projectPath)
                .Where(path => Path.GetExtension(path).ToLower() == ".json");
            return jsonFiles
                .Where(path => HasConventionalName(path) || ReferencesArmSchema(path));
        }

        /// <summary>
        /// Return true if named according to conventional name: azuredeploy.json
        /// </summary>
        private static bool HasConventionalName(string path)
        {
            return Path.GetFileName(path).Equals(AzureDeployJson, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Look for $schema reference that matches ARM template schema
        /// </summary>
        private static bool ReferencesArmSchema(string path)
        {
            // HACK: Peeking at first two lines to check for ARM schema URL
            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                var text = reader.ReadLine() + reader.ReadLine();
                return text.Contains(TemplateJsonSchema);
            }
        }
    }
}
