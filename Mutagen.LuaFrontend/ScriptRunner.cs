using MicroIOC;
using MutagenRuntime;
using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend
{
    public class ScriptRunner
    {
        [MuImport]
        ApiAdapter api;

        [MuImport]
        Lua myLua;

        LuaGlobalPortable luaEnv;
        LuaChunk myChunk;

        public ScriptRunner()
        {            
        }

        public void Load(Stream scriptSource)
        {
            api.Init();
            luaEnv = myLua.CreateEnvironment();
            myChunk = myLua.CompileChunk(new StreamReader(scriptSource), "__mutagenScript", null);
        }

        public List<TestResult> Run()
        {
            LuaUtil.PublishObjectMethods(api, luaEnv);
            api.LuaEnv = luaEnv;
            luaEnv.DoChunk(myChunk);
            api.ExecTestCase();
            return api.TestResults();
        }

    }
}
