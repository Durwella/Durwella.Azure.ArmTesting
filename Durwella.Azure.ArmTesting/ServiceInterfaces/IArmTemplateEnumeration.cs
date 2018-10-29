using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.ServiceInterfaces
{
    public interface IArmTemplateEnumeration
    {
        /// <summary>
        /// Searches Project directory recursively for all *.json files that smell like ARM templates
        /// </summary>
        IEnumerable<string> EnumerateArmTemplatePaths(string projectPath);
    }
}