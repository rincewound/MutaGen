﻿using System;
using MutagenRuntime;
using System.Collections.Generic;
using NUnit.Framework;
using System.Collections;

namespace MutagenTests
{
    [TestFixture]
    public class TestEnvironmentTests
    {
        TestEnvironment te;

        [SetUp]
        public void Init()
        {
            te = new TestEnvironment();
        }

        [Test]
        public void GetFacette_ReturnsFacette()
        {
            var f = new Facette("fnord", new List<object>() { 2 });
            te.AddFacette(f);
            var returned = te.GetFacette("fnord");
        }

        [Test]
        public void GetFacette_ThowsNoSuchFacetteException_IfFacetteWasNotAdded()
        {
            try
            {
                te.GetFacette("notcreated");
                Assert.Fail("did not throw!");
            }
            catch (MutagenRuntime.NoSuchFacetteException)
            {

            }
        }


        [Test]
        public void CreateBindings_ReturnsBinding()
        {
            var f = new Facette("fnord", new List<object> { 1, 2, 3 });
            te.AddFacette(f);

            var testContext = new MutagenRuntime.TestContext();
            testContext.AddFacette("fnord", 1, 1);

            var theBinding = te.CreateBindings(testContext);
            Assert.IsNotNull(theBinding);
            Assert.AreEqual(3, theBinding.Count);
        }

        [Test]
        public void CreateBindings_SetsFacetteValue()
        {
            var f = new Facette("fnord", new List<object> { 1, 2, 3 });
            te.AddFacette(f);

            var testContext = new MutagenRuntime.TestContext();
            testContext.AddFacette("fnord", 1, 1);

            var theBinding = te.CreateBindings(testContext);
            Assert.IsNotNull(theBinding[0].theFacette);
        }

        [Test]
        public void CreateBindings_ReturnsBinding_MultipleFacettes()
        {
            var f1 = new Facette("fnord1", new List<object> { 1, 2, 3 });
            var f2 = new Facette("fnord2", new List<object> { 4, 5, 6 });
            var f3 = new Facette("fnord3", new List<object> { 7, 8, 9 });
            te.AddFacette(f1);
            te.AddFacette(f2);
            te.AddFacette(f3);

            var testContext = new MutagenRuntime.TestContext();
            testContext.AddFacette("fnord1", 1, 1);
            testContext.AddFacette("fnord2", 1, 1);
            testContext.AddFacette("fnord3", 1, 1);

            var theBinding = te.CreateBindings(testContext);
            Assert.IsNotNull(theBinding);
            Assert.AreEqual(27, theBinding.Count);
        }

        [Test]
        public void CreateBindings_CanNavigateFacettes()
        {
            var f1 = new Facette("fnord1", new List<object> { 1, 2, 3 });
            var f2 = new Facette("fnord2", new List<object> { 4, 5, 6 });
            var f3 = new Facette("fnord3", new List<object> { 7, 8, 9 });
            te.AddFacette(f1);
            te.AddFacette(f2);
            te.AddFacette(f3);

            var testContext = new MutagenRuntime.TestContext();
            testContext.AddFacette("fnord1", 1, 1);
            testContext.AddFacette("fnord2", 1, 1);
            testContext.AddFacette("fnord3", 1, 1);

            var theBinding = te.CreateBindings(testContext);
            // Bindings are organised as a singly linked list, so we
            // should be able to access the last item in the list (3 facettes mean, that we
            // always get a list of three entries)
            Assert.IsNull(theBinding[0].next.next.next);
        }


        [Test]
        public void CreateBindings_CreatesCorrectValues()
        {
            var f1 = new Facette("fnord1", new List<object> { 1, 2 });
            var f2 = new Facette("fnord2", new List<object> { 4, 5 });

            te.AddFacette(f1);
            te.AddFacette(f2);            

            var testContext = new MutagenRuntime.TestContext();
            testContext.AddFacette("fnord1", 1, 1);
            testContext.AddFacette("fnord2", 1, 1);

            var theBinding = te.CreateBindings(testContext);
            // Bindings are organised as a singly linked list, so we
            // should be able to access the last item in the list (3 facettes mean, that we
            // always get a list of three entries)

            var B01 = new BitArray(new[] { false, true });
            var B10 = new BitArray(new[] { true, false });

            Assert.IsTrue(theBinding[0].valueSet.SameAs(B10));
            Assert.IsTrue(theBinding[0].next.valueSet.SameAs(B10));

            Assert.IsTrue(theBinding[1].valueSet.SameAs(B10));
            Assert.IsTrue(theBinding[1].next.valueSet.SameAs(B01));

            Assert.IsTrue(theBinding[2].valueSet.SameAs(B01));
            Assert.IsTrue(theBinding[2].next.valueSet.SameAs(B10));

            Assert.IsTrue(theBinding[3].valueSet.SameAs(B01));
            Assert.IsTrue(theBinding[3].next.valueSet.SameAs(B01));

        }

    }
}
