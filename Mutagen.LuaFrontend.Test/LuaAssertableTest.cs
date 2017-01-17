using MutagenRuntime;
using Neo.IronLua;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend.Test
{
    [TestFixture]
    class LuaAssertableTest
    {
        Lua lua;
        LuaGlobalPortable luaEnv;
        LuaChunk scriptChunk;

        [SetUp]
        public void Setup()
        {
            lua = new Lua();
            luaEnv = lua.CreateEnvironment();
        }

        private void ReadScript(string path)
        {
            var tc = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
            var data = System.IO.File.ReadAllText(tc + path);
            scriptChunk = lua.CompileChunk(data, "scriptChunk", null);
        }

        [Test]
        public void Assertable_CanBeCreated()
        {
            IAssertable ass = new LuaAssertable(lua, luaEnv, scriptChunk);
        }

        [Test]
        public void Execute_CallsLuaExecute()
        {                                               
            ReadScript("./LuaScripts/Assertable.lua");
            var ass = new LuaAssertable(lua, luaEnv, scriptChunk);
            Assert.True(ass.Execute()[0].result);
        }


        [Test]
        public void Execute_MultipleAssertsAreLogged()
        {
            ReadScript("./LuaScripts/Assertable_MultipleAsserts.lua");
            var ass = new LuaAssertable(lua, luaEnv, scriptChunk);
            var res = ass.Execute();

            Assert.AreEqual(3,     res.Count);
            Assert.AreEqual(true,  res[0].result);
            Assert.AreEqual(false, res[1].result);
            Assert.AreEqual(true,  res[2].result);
        }

        class AssertWidget
        {
            public bool ASW_Assert(bool val)
            {
                return val;
            }
        }

        [Test]
        public void Execute_CanCallToOtherObject()
        {
            AssertWidget w = new AssertWidget();
            ReadScript("./LuaScripts/Assertable_CallOtherObject.lua");
            var ass = new LuaAssertable(lua, luaEnv, scriptChunk);
            LuaUtil.PublishObjectMethods(w, luaEnv);
            var res = ass.Execute();
            Assert.AreEqual(2,     res.Count);
            Assert.AreEqual(false, res[0].result);
            Assert.AreEqual(true,  res[1].result);
        }

    }
}
