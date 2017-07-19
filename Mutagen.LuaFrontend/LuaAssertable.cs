using MutagenRuntime;
using Neo.IronLua;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend
{
    public class LuaAssertable : IAssertable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        dynamic luaEnv;
        List<AssertResult> res = new List<AssertResult>();

        public LuaAssertable( dynamic env)
        {
            if (env == null)
                throw new InvalidOperationException("Env instance must not be NULL");
            luaEnv = env;
        }

        public List<AssertResult> Execute()
        {
            if (luaEnv["ExecuteTest"] == null)
                throw new InvalidOperationException("env has no method called ExecuteTest. Don't know what to do with this.");

            LuaUtil.PublishObjectMethods(this, luaEnv);
            res.Clear();
            luaEnv.ExecuteTest();
            return res;
        }

        public void __ASSERT(bool value)
        {
            if (!value)
            {
                logger.Error("Assert failed on __ASSERT " + (res.Count + 1));
                //luaEnv.DoChunk("debug.traceback(\"Stack trace\")", null);
            }
            res.Add(new AssertResult { result = value, info = "LuaAssert " + (res.Count + 1) });
        }
    }
}
