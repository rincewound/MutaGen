using System;
using System.Text;
using System.Collections.Generic;
using MutagenRuntime;
using System.Linq;
using NUnit.Framework;

namespace MutagenTests
{
    [TestFixture]
    public class AssertCollectionTests
    {

        AssertCollection theCollection;

        [SetUp]
        public void Setup()
        {
            theCollection = new AssertCollection();
        }
       
        [Test]
        public void EmptyCollection_ThrowsOnExecute()
        {
            try
            {
                theCollection.Execute();
                Assert.Fail("AssertCollection did not throw on execute!");
            }
            catch
            {

            }
        }

        [Test]
        public void Execute_ExecutesSingleStep()
        {
            bool stepSet = false;

            theCollection.Push(() => { stepSet = true; return true; });
            theCollection.Execute();
            Assert.IsTrue(stepSet);
        }

        [Test]
        public void Execute_ExecutesAllSteps()
        {
            int numSteps = 0;

            for(int i = 0; i < 123; i++)
                theCollection.Push(() => { numSteps++; return true; });
            theCollection.Execute();
            Assert.AreEqual(123, numSteps);
        }

        [Test]
        public void Execute_ReturnsCorrectResult()
        {
            int nSteps = 0;

            for (int i = 0; i < 10; i++)
                theCollection.Push(() => { nSteps++; return nSteps % 2 == 0; });
            var result = theCollection.Execute();

            Assert.AreEqual(5, result.Count(x => x.result == true));
            Assert.AreEqual(5, result.Count(x => x.result == false));
        }

        [Test]
        public void GuardedAssert_DoesNotCallGuardedFuncIfGuardFailsToTrigger()
        {
            bool funcCalled = false;
            var ass = AssertDelegate.GuardedAssert(() => { funcCalled = true; return true; }, () => false);

            Assert.IsTrue(ass());
            Assert.IsFalse(funcCalled);
        }

        [Test]
        public void GuardedAssert_CallsGuardedFuncIfGuardIsTriggered()
        {
            var funcCalled = false;
            var ass = AssertDelegate.GuardedAssert(() => { funcCalled = true; return true; }, () => true);

            Assert.IsTrue(ass());
            Assert.IsTrue(funcCalled);
        }

        [Test]
        public void GuardedAssert_ReturnsValueOfCalledFunction()
        {
            var ass = AssertDelegate.GuardedAssert(() => { return false; }, () => true);
            Assert.IsFalse(ass());            
        }

    }
}
