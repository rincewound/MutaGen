using System;
using NUnit.Framework;
using Neo.IronLua;
using MicroIOC;
using Simple.Mocking;
using MutagenRuntime;

namespace Mutagen.LuaFrontend.Test
{

    [TestFixture]
    public class FrontEndApiTest
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
            apiBridge = Mock.Interface<IApiBridge>();
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
        public void BeginTestCase_CallsApi()
        {           
            Expect.Once.MethodCall(() => apiBridge.BeginTestCase("TestHarnessTheFirst", "TstAssembly.dll"));

            ApiAdapter api = IOC.Resolve<ApiAdapter>();
            ReadScript("./LuaScripts/BeginTestCase.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);
            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
        }

        [Test]
        public void CallTo_CreateFacette_CallsApi()
        {
            Expect.Once.MethodCall(() => apiBridge.CreateFacette("facName", Any<System.Collections.Generic.List<object>>.Value));

            ApiAdapter api = IOC.Resolve<ApiAdapter>();
            ReadScript("./LuaScripts/CreateFacette.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);            
        }

        [Test]
        public void CallTo_AddFacette_CallsApi()
        {
            Expect.Once.MethodCall(() => apiBridge.AddFacette("fnord", 1, 7));

            ApiAdapter api = IOC.Resolve<ApiAdapter>();
            ReadScript("./LuaScripts/AddFacette.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);            
        }
    }
}
