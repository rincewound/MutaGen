using System;
using NUnit.Framework;
using Neo.IronLua;
using MicroIOC;
using Simple.Mocking;
using MutagenRuntime;

namespace Mutagen.LuaFrontend.Test
{

    [TestFixture]
    public class ApiAdapterTests
    {
        Lua lua;
        LuaGlobalPortable luaEnv;
        LuaChunk scriptChunk;
        IApiBridge apiBridge;
        ApiAdapter api;

        [SetUp]
        public void Setup()
        {
            IOC.Reset();
            lua = new Lua();
            luaEnv = lua.CreateEnvironment();
            apiBridge = Mock.Interface<IApiBridge>();
            IOC.Register<IApiBridge>( () => apiBridge);
            IOC.Register<ApiAdapter>( () => new ApiAdapter());
            IOC.Register<LuaGlobalPortable>(() => luaEnv);
            api = IOC.Resolve<ApiAdapter>();
            //api.LuaEnv = luaEnv;
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
            Expect.Once.MethodCall(() => apiBridge.BeginTestCase("SimpleHarness", "Mutagen.LuaFrontend.Test.dll"));
            Expect.Once.MethodCall(() => apiBridge.TestHarness()).Returns(new SimpleHarness());
            ReadScript("./LuaScripts/BeginTestCase.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);
            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
        }

        [Test]
        public void BeginTestCase_MakesTestharnessAvailableToScript()
        {
            var myHarness = new SimpleHarness();
            Expect.Once.MethodCall(() => apiBridge.BeginTestCase("SimpleHarness", "Mutagen.LuaFrontend.Test.dll"));
            Expect.Once.MethodCall(() => apiBridge.TestHarness()).Returns(myHarness);
            ReadScript("./LuaScripts/BeginTestCase_UseHarness.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);            
            luaEnv.DoChunk(scriptChunk);
            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
            Assert.AreEqual(myHarness.lastPrint, "Test");
        }

        [Test]
        public void CallTo_CreateFacette_CallsApi()
        {
            Expect.Once.MethodCall(() => apiBridge.CreateFacette("facName", Any<System.Collections.Generic.List<object>>.Value));

            ReadScript("./LuaScripts/CreateFacette.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);

            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
        }

        [Test]
        public void CallTo_AddFacette_CallsApi()
        {
            Expect.Once.MethodCall(() => apiBridge.AddFacette("fnord", 1, 7));

            ReadScript("./LuaScripts/AddFacette.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);

            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
        }

        [Test]
        public void CallTo_EndTestCase_CallsApi()
        {
            Expect.Once.MethodCall(() => apiBridge.CommitTestCaseCode(Any<IAssertable>.Value.Matching(x => x != null).AsInterface));

            //api.LuaEnv = luaEnv;
            ReadScript("./LuaScripts/EndTestCase.lua");            
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);

            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);            
        }

        [Test]
        public void CallTo_CreateConstraint_CallsApi()
        {
            Expect.Once.MethodCall(() => apiBridge.CreateFacette("F01", Any<System.Collections.Generic.List<object>>.Value));
            Expect.Once.MethodCall(() => apiBridge.CreateFacette("F02", Any<System.Collections.Generic.List<object>>.Value));
            //Expect.Once.MethodCall(() => apiBridge.CreateConstraint("F01", "F02", "F01Constraint", Any<System.Collections.Generic.List<object>>.Value);
            //api.LuaEnv = luaEnv;
            ReadScript("./LuaScripts/AddConstraint.lua");
            LuaUtil.PublishObjectMethods(api, luaEnv);
            luaEnv.DoChunk(scriptChunk);

            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);


            Assert.Fail("Test does not check call to api.CreateConstraint!");

        }
    }
}
