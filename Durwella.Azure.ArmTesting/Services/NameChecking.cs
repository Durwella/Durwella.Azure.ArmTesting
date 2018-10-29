using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.Services
{
    public class NameChecking
    {
        public IEnumerable<ArmTemplateError> CheckResourceNames(string armTemplateJson)
        {
            return new List<ArmTemplateError>();
        }
    }
}
