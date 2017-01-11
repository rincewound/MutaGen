using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend
{
    public interface IApiBridge
    {
        void CreateFacette(string facetteName, List<object> values);

        void BeginTestCase(string usedHarness, string assemblyName);
        void AddFacette(string facetteName, int minValues, int maxValues);
        //void CommitTestCaseCode(IAssertable code)
        void ExecTestCase();
    }
}
