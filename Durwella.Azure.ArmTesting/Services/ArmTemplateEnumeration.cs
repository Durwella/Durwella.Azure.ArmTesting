using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Durwella.Azure.ArmTesting.Services
{
    public class ArmTemplateEnumeration
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
        /// https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates#template-format
        /// </summary>
        const string TemplateJsonSchema = "http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json";

        public IEnumerable<string> EnumerateArmTemplatePaths(string projectPath)
        {
            var directory = Path.GetDirectoryName(projectPath);
            //var azureDeployPaths = Directory.EnumerateFiles(directory, AzureDeployJson, SearchOption.AllDirectories);
            //var jsonPaths = EnumerateJsonFilesWithArmSchema(directory);
            //return azureDeployPaths.Union(jsonPaths);
            return EnumerateJsonFilesWithArmSchema(directory);
        }

        /// <summary>
        /// Searches directory recursively for all *.json files that smell like ARM templates
        /// </summary>
        private static IEnumerable<string> EnumerateJsonFilesWithArmSchema(string directory)
        {
            var jsonFiles = Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories);
            foreach (var path in jsonFiles)
            {
                // If it is named according to convention, assume it is an ARM template
                if (Path.GetFileName(path).Equals(AzureDeployJson, StringComparison.OrdinalIgnoreCase))
                    yield return path;
                // HACK: Peeking at first two lines to check for ARM schema URL
                using (var reader = new StreamReader(File.OpenRead(path)))
                {
                    var text = reader.ReadLine() + reader.ReadLine();
                    if (text.Contains(TemplateJsonSchema))
                        yield return path;
                }
            }
        }
    }
}
