using System;
using MutagenRuntime;
using System.Collections.Generic;
using MicroIOC;
using NUnit.Framework;

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

        [SetUp]
        public void Init()
        {
            IOC.Reset();
            IOC.Register<TestEnvironment>(() => { return ted; });
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
    }
}
