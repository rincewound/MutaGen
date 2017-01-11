using System;
using NUnit.Framework;
using Neo.IronLua;
using FakeItEasy;
using MicroIOC;

namespace Mutagen.LuaFrontend.Test
{
    [TestFixture]
    public class UnitTest1
    {
        Lua lua;
        LuaGlobalPortable luaEnv;
        LuaChunk scriptChunk;
        IApiBridge apiBridge;

        [SetUp]
        public void Setup()
        {
            IOC.Reset();
            lua = new Lua();
            luaEnv = lua.CreateEnvironment();
            apiBridge = A.Fake<IApiBridge>();
            IOC.Register<IApiBridge>( () => apiBridge);
            IOC.Register<ApiAdapter>(() => new ApiAdapter());
        }

        private void ReadScript(string path)
        {
            var tc = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
            var data = System.IO.File.ReadAllText(tc + path);
            scriptChunk = lua.CompileChunk(data, "scriptChunk", null);
        }

        [TearDown]
        public void Teardown()
        {
            luaEnv = null;
            lua.Dispose();
        }

        [Test]
        public void CallTo_CreateFacette_CallsApi()
        {
            ApiAdapter api = IOC.Resolve<ApiAdapter>();
            ReadScript("./LuaScripts/CreateFacette.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);
            A.CallTo(() => apiBridge.CreateFacette("facName", A<System.Collections.Generic.List<object> >.Ignored)).MustHaveHappened();
        }

        [Test]
        public void CallTo_AddFacette_CallsApi()
        {
            ApiAdapter api = IOC.Resolve<ApiAdapter>();
            ReadScript("./LuaScripts/AddFacette.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);
            A.CallTo(() => apiBridge.AddFacette("fnord", 1, 7)).MustHaveHappened();
        }
    }
}
