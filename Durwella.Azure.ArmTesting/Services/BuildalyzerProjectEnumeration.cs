using Durwella.Azure.ArmTesting.ServiceInterfaces;
using System;
using System.Collections.Generic;

namespace Durwella.Azure.ArmTesting.Services
{
    public class BuildalyzerProjectEnumeration : IProjectFileEnumeration
    {
        public IEnumerable<string> EnumerateProjectFiles(string projectPath)
        {
            throw new NotImplementedException();
        }
    }
}
