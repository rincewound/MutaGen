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
            var hr = new SimpleHarness();
            runner = new ScriptRunner();
            
            IOC.ResolveImports(runner);

            Expect.MethodCall(() => apiBridge.Init());

            runner.Load(strm);

            Expect.MethodCall(() => apiBridge.BeginTestCase("SimpleHarness", "Mutagen.FrontEnd.Test.dll"));
            Expect.Once.MethodCall(() => apiBridge.TestResults()).Returns(new List<TestResult>());
            Expect.Once.MethodCall(() => apiBridge.TestHarness()).Returns(hr);

            Expect.Once.MethodCall(() => apiBridge.CommitTestCaseCode(Any<IAssertable>.Value.Matching(x => x != null).AsInterface));
            Expect.Once.MethodCall(() => apiBridge.ExecTestCase());

            runner.Run();

            AssertInvocationsWasMade.MatchingExpectationsFor(apiBridge);
        }

        [Test]
        public void Run_FullTestCase()
        {
            IOC.Reset();
            lua = new Lua();
            apiBridge = new DefaultApiBridge();
            IOC.Register<IApiBridge>(() => apiBridge);
            IOC.Register<ITestEnvironment>(() => new TestEnvironment());
            IOC.Register<ITestContext>(() => new MutagenRuntime.TestContext());
            IOC.Register<Lua>(() => lua);
            IOC.Register<ApiAdapter>(() => new ApiAdapter());


            var tc = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
            var strm = System.IO.File.OpenRead(tc + "./LuaScripts/FullTestCase.lua");
            runner = new ScriptRunner();
            IOC.ResolveImports(runner);

            // Black Magic: NUnit sets the working dir to a crappy place,
            // so we're shit out of luck, if we try to access a file relative to
            // our working dir. To get around this problem we magic the correct
            // path here and reset the working directory to reflect this:
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Environment.CurrentDirectory = dir;

            runner.Load(strm);

            runner.Run();

            var results = Api.GetResults();

            Assert.AreEqual(6, results.Count);
        }

    }
}
