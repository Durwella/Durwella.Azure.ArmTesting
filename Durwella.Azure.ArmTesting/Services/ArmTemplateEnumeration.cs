using System.Collections.Generic;
using System.IO;

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

        public IEnumerable<string> EnumerateArmTemplatePaths(string projectPath)
        {
            var directory = Path.GetDirectoryName(projectPath);
            return Directory.EnumerateFiles(directory, AzureDeployJson, SearchOption.AllDirectories);
        }
    }
}
