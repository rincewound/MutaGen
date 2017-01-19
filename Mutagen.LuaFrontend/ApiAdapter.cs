using MicroIOC;
using MutagenRuntime;
using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend
{
    public class ApiAdapter 
    {
        [MuImport]
        IApiBridge bridge;

        public LuaGlobalPortable LuaEnv { get; set; }

        public void CreateFacette(string facetteName, LuaTable values)
        {
            var lst = new List<object>();
            // we start at 1 b/c Lua arrays are 1 based!
            for (int i = 1; i <= values.Length; i++)
                lst.Add(values[i]);

            bridge.CreateFacette(facetteName, lst);
        }

        public void Init()
        {
            bridge.Init();
        }

        public void AddFacette(string facetteName, int minValues, int maxValues)
        {
            bridge.AddFacette(facetteName, minValues, maxValues);
        }

        internal void ExecTestCase()
        {
            bridge.ExecTestCase();
        }

        public void BeginTestCase(string harnessName, string assemblyName)
        {
            bridge.BeginTestCase(harnessName, assemblyName);
            var theHarness = bridge.TestHarness();
            LuaUtil.PublishObjectMethods(theHarness, LuaEnv);
        }

        public void CommitTestCaseCode()
        {
            // Build IAssertable from TCFunc
            var luaAssertable = new LuaAssertable(LuaEnv);
            bridge.CommitTestCaseCode(luaAssertable);
        }

        public ITestHarness Testharness()
        {
            return Api.Testharness();
        }    
    }
}
