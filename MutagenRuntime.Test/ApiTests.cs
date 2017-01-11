using System;
using MutagenRuntime;
using System.Collections.Generic;
using MicroIOC;
using NUnit.Framework;
using FakeItEasy;

namespace MTest
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

        [Ignore("Not testable this way")]
        public void BeginTestCase_CanUseSimpleNameIfAssemblynameIsGiven()
        {
            Api.BeginTestCase("MTest.IAmAHarness", @"./MutagenRuntime.Test.dll");

            Assert.IsNotNull(Api.Testharness());
        }

        [Ignore("Not testable this way")]
        public void BeginTestCase_ClearsTestContext()
        {
            // Can we test this at all?
            Assert.Fail("Implement");
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

    }
}
