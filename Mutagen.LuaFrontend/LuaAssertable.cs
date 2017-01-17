using MutagenRuntime;
using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend
{
    public class LuaAssertable : IAssertable
    {
        Lua lua;
        dynamic luaEnv;
        LuaChunk scriptChunk;

        List<AssertResult> res = new List<AssertResult>();

        public LuaAssertable(Lua lua, dynamic env, LuaChunk scriptChnk)
        {
            this.lua = lua;
            this.luaEnv = env;
            this.scriptChunk = scriptChnk;
            luaEnv.dochunk(scriptChunk);
        }

        public List<AssertResult> Execute()
        {
            LuaUtil.PublishObjectMethods(this, luaEnv);
            res.Clear();
            luaEnv.ExecuteTest();
            return res;
        }

        public void __ASSERT(bool value)
        {
            res.Add(new AssertResult { result = value, info = "LuaAssert" });
        }
    }
}
