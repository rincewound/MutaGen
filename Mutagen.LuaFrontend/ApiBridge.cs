using MicroIOC;
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

        public void CreateFacette(string facetteName, LuaTable values)
        {
            var lst = new List<object>();
            // we start at 1 b/c Lua arrays are 1 based!
            for (int i = 1; i <= values.Length; i++)
                lst.Add(values[i]);

            bridge.CreateFacette(facetteName, lst);
        }

        public void AddFacette(string facetteName, int minValues, int maxValues)
        {
            bridge.AddFacette(facetteName, minValues, maxValues);
        }

        public void BeginTestCase(string HarnessName, string AssemblyName)
        {

        }
    }
}
