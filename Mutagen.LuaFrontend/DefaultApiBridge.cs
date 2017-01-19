using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MutagenRuntime;

namespace Mutagen.LuaFrontend
{
    public class DefaultApiBridge : IApiBridge
    {
        public void AddFacette(string facetteName, int minValues, int maxValues)
        {
            Api.AddFacette(facetteName, minValues, maxValues);
        }

        public void BeginTestCase(string usedHarness, string assemblyName)
        {
            Api.BeginTestCase(usedHarness, assemblyName);
        }

        public void CommitTestCaseCode(IAssertable tcCode)
        {
            Api.CommitTestCaseCode(tcCode);
        }

        public void CreateFacette(string facetteName, List<object> values)
        {
            Api.CreateFacette(facetteName, values);
        }

        public void ExecTestCase()
        {
            Api.ExecTestCase();
        }

        public void Init()
        {
            Api.Init();
        }

        public ITestHarness TestHarness()
        {
            return Api.Testharness();
        }
    }
}
