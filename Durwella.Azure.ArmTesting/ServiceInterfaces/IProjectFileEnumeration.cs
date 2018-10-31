using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.ServiceInterfaces
{
    public interface IProjectFileEnumeration
    {
        IEnumerable<string> EnumerateProjectFiles(string projectPath);
    }
}