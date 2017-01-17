using MicroIOC;
using MutagenRuntime;
using Neo.IronLua;
using NUnit.Framework;
using Simple.Mocking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend.Test
{
    [TestFixture]
    public class ScriptRunnerTests
    {
        IApiBridge apiBridge;
        Lua lua;
        ScriptRunner runner;

        [SetUp]
        public void Setup()
        {
           IOC.Reset();
           lua = new Lua();
           apiBridge = Mock.Interface<IApiBridge>();
           IOC.Register<IApiBridge>(() => apiBridge);
           IOC.Register<Lua>(() => lua);
           IOC.Register<ApiAdapter>(() => new ApiAdapter());           
        }

        [Test]
        public void Run_CallsApi()
        {
            var tc = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
            var strm = System.IO.File.OpenRead(tc + "./LuaScripts/SimpleTestCase.lua");
            runner = new ScriptRunner();
            
            IOC.ResolveImports(runner);
            runner.Load(strm);

            Expect.Once.MethodCall(() => apiBridge.CommitTestCaseCode(Any<IAssertable>.Value.Matching(x => x != null).AsInterface));
            Expect.Once.MethodCall(() => apiBridge.ExecTestCase());

            runner.Run();

            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
        }

    }
}
