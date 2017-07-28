using System;
using MutagenRuntime;
using System.Collections.Generic;
using MicroIOC;
using NUnit.Framework;
using FakeItEasy;

namespace MutagenTests
{

    public class IAmAHarness : ITestHarness
    {
        public void AddFacette(Facette f)
        {
            throw new NotImplementedException();
        }

        public void AddFacetteSlice()
        {
            throw new NotImplementedException();
        }

        public void AddMultiSelect()
        {
            throw new NotImplementedException();
        }

        public List<object> GetFacetteSelection()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class ApiTests
    {
        class TestEnvDummy : TestEnvironment
        {
            public bool addCalled = false;
            public override void AddFacette(Facette f)
            {
                addCalled = true;
                base.AddFacette(f);
            }

        }

        TestEnvDummy ted = new TestEnvDummy();
        ITestEnvironment fakeTev = A.Fake<ITestEnvironment>();

        [SetUp]
        public void Init()
        {
            IOC.Reset();
            IOC.Register<ITestEnvironment>(() => {
                ted = new TestEnvDummy();
                return ted; });
            IOC.Register<ITestContext>(() =>  new MutagenRuntime.TestContext());
            Api.Init();
        }

        [Test]
        public void CreateFacette_CreatesFacette()
        {
            Api.CreateFacette("fnord", new System.Collections.Generic.List<object>() { 1 });
            Assert.IsTrue(ted.addCalled);
        }

        [Test]
        public void BeginTestCase_ThrowsIfTypeNotFound()
        {
            try
            {
                Api.BeginTestCase("NotAType", null);
                Assert.Fail("BeginTestCase did not throw.");
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void BeginTestCase_ThrowsIfTypeIsNotAHarness()
        {
            try
            {
                Api.BeginTestCase("MutagenRuntime.Api", null);
                Assert.Fail("BeginTestCase did not throw.");
            }
            catch (InvalidCastException)
            {


            }
        }

        [Test]
        public void BeginTestCase_DoesNotThrowIfTypeIsAHarness()
        {
            var n = typeof(IAmAHarness);           
            Api.BeginTestCase(n.AssemblyQualifiedName, null);
        }

        [Test]
        public void BeginTestCase_CanUseSimpleNameIfAssemblynameIsGiven()
        {
            // Black Magic: NUnit sets the working dir to a crappy place,
            // so we're shit out of luck, if we try to access a file relative to
            // our working dir. To get around this problem we magic the correct
            // path here and reset the working directory to reflect this:
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Environment.CurrentDirectory = dir;

            Api.BeginTestCase("MutagenTests.IAmAHarness", @"./MutagenRuntime.Test.dll");

            Assert.IsNotNull(Api.Testharness());
        }

        [Test]
        public void AddFacette_AddsFacetteToCurrentTestContext()
        {
            var theContext = A.Fake<ITestContext>();
            IOC.Reset();
            IOC.Register<ITestEnvironment>(() => fakeTev);
            IOC.Register<ITestContext>(() => theContext);

            var n = typeof(TestyHarness);
            Api.CreateFacette("TestFac", new List<object>() { 1, 2, 3 });
            Api.BeginTestCase(n.AssemblyQualifiedName, null);
            Api.AddFacette("TestFac", 1, 2);

            A.CallTo(() => theContext.AddFacette("TestFac",1, 2)).MustHaveHappened();
        }

        [Test]
        public void AddFacette_Throws_IfNoTestCaseWasBegun()
        {
            var n = typeof(TestyHarness);
            IOCSetup();

            try
            {
                Api.CreateFacette("TestFac", new List<object>() { 1, 2, 3 });
                Api.AddFacette("TestFac", 1, 1);
                Assert.Fail("AddFacette did not throw, when no testcase was started.");
            }
            catch (NoTestCaseBegunException)
            {

            }

        }

        class TestyHarness: IAmAHarness
        {
            public static List<List<object>> valueSets = new List<List<object>>();
            virtual public void ApplyTestFac(List<object> data)
            {
                valueSets.Add(data);
            }

            virtual public void ApplyTestFac2(List<object> data)
            {
                valueSets.Add(data);
            }
        }

        private void IOCSetup()
        {
            IOC.Reset();
            IOC.Register<ITestEnvironment>(() => fakeTev);
            IOC.Register<ITestContext>(() => new MutagenRuntime.TestContext());
        }

        [Test]
        public void ExecTestCase_Throws_NoTestCaseException_IfNoAssertableIsAvailable()
        {
            IOCSetup();

            try
            {
                Api.ExecTestCase();
                Assert.Fail("Did not throw execption!");
            }
            catch (NoTestCaseException)
            {
                // All good!
                return;
            }
        }

        [Test]
        public void ExecTestCase_Throws_NoTestCaseStartedException_IfNoTestCaseWasBegun()
        {
            var n = typeof(TestyHarness);
            IOCSetup();

            try
            {
                var coll = new AssertCollection();
                coll.Push(() => true);
                Api.CommitTestCaseCode(coll);
                Api.ExecTestCase();
                Assert.Fail("Did not throw execption!");
            }
            catch (NoTestCaseStartedException)
            {
                // All good!
                return;
            }
        }

        [Test]
        public void ExecTestCase_AppliesBindings()
        {
            var n = typeof(TestyHarness);
            IOCSetup();

            Api.CreateFacette("TestFac", new List<object>() { 1, 2, 3 });
            Api.BeginTestCase(n.AssemblyQualifiedName, null);
            Api.AddFacette("TestFac", 1, 1);

            var coll = new AssertCollection();
            coll.Push(() => true);
            Api.CommitTestCaseCode(coll);
            Api.ExecTestCase();

            Assert.AreEqual(3, TestyHarness.valueSets.Count);
        }

        [Test]
        public void ExecTestCase_AppliesCorrectNumberOfBindings()
        {
            var n = typeof(TestyHarness);
            TestyHarness.valueSets.Clear();
            IOCSetup();

            Api.CreateFacette("TestFac", new List<object>() { 1, 2, 3 });
            Api.BeginTestCase(n.AssemblyQualifiedName, null);
            Api.AddFacette("TestFac", 1, 2);

            var coll = new AssertCollection();
            coll.Push(() => true);
            Api.CommitTestCaseCode(coll);
            Api.ExecTestCase();

            Assert.AreEqual(6, TestyHarness.valueSets.Count);
        }

        [Test]
        public void ExecTestCase_AppliesCorrectNumberOfBindingsMultipleFacettes()
        {
            var n = typeof(TestyHarness);
            TestyHarness.valueSets.Clear();
            IOCSetup();

            Api.CreateFacette("TestFac", new List<object>() { 1, 2, 3 });
            Api.CreateFacette("TestFac2", new List<object>() { 4, 5, 6 });
            Api.BeginTestCase(n.AssemblyQualifiedName, null);
            Api.AddFacette("TestFac", 1, 2);
            Api.AddFacette("TestFac2", 1, 1);

            var coll = new AssertCollection();
            coll.Push(() => true);
            Api.CommitTestCaseCode(coll);
            Api.ExecTestCase();

            var res = Api.GetResults();

            Assert.AreEqual(18, res.Count);
        }

        [Test]
        public void ExecTestCase_ReturnsCorrectNumberOfResults()
        {
            var n = typeof(TestyHarness);
            TestyHarness.valueSets.Clear();
            IOCSetup();

            Api.CreateFacette("TestFac", new List<object>() { 1, 2, 3 });
            Api.BeginTestCase(n.AssemblyQualifiedName, null);
            Api.AddFacette("TestFac", 1, 2);

            var coll = new AssertCollection();
            coll.Push(() => true);
            Api.CommitTestCaseCode(coll);
            Api.ExecTestCase();

            var res = Api.GetResults();

            Assert.AreEqual(6, res.Count);
        }

        [Test]
        public void Api_CreateConstraint_CallsTestEnv()
        {

            var theContext = A.Fake<ITestContext>();
            IOC.Reset();
            IOC.Register<ITestEnvironment>(() => fakeTev);
            IOC.Register<ITestContext>(() => theContext);

            A.CallTo(() => fakeTev.GetFacette(A<string>.Ignored)).Returns(new Facette("x", new List<object> { 1, 2 }));

            Api.Init();
            Api.CreateFacette("F01", new List<object> { 1 });
            Api.CreateFacette("F02", new List<object> { 1 });
            Api.CreateConstraint("F01", "F02", x=>true, null);
            A.CallTo(() => fakeTev.AddConstraint(A<Constraint>.Ignored)).MustHaveHappened();
        }


        
    }
}
