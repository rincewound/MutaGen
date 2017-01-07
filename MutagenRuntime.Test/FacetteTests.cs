
using MutagenRuntime;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTest
{
    [TestFixture]
    public class FacetteTests
    {
        [Test]
        public void Facette_Ctor_TakesList()
        {
            var fac = new Facette("Facetto", new List<object> { 1, 2, 3, "foo" });
        }

        [Test]
        public void Facette_Ctor_ThrowsIfListIsNull()
        {
            try
            {
                var fac = new Facette("Facetto", null);
                Assert.Fail("Facette Ctor did not throw when handed a NULL list");
            }
            catch
            {

            }            
        }

        [Test]
        public void Facette_Ctor_ThrowsIfListIsEmpty()
        {
            try
            {
                var fac = new Facette("Facetto", new List<object>());
                Assert.Fail("Facette Ctor did not throw when handed an empty list");
            }
            catch
            {

            }
        }

        [Test]
        public void Facette_NextVal_IteratesAllValues()
        {
            var fac = new Facette("Facetto", new List<object> { 1, 2, 3, "foo" });
            Assert.AreEqual(1, (int) fac.NextValue());
            Assert.AreEqual(2, (int)fac.NextValue());
            Assert.AreEqual(3, (int)fac.NextValue());
            Assert.AreEqual("foo", fac.NextValue());
            Assert.AreEqual(null, fac.NextValue());
        }

        [Test]
        public void Facette_GetValidCombinations_SelectsAllBits_SingleBit()
        {
            List<BitArray> allCombinations = new List<BitArray>
            {
                new BitArray(new[] { true, false, false }),   // b001
                new BitArray(new[] { false, true, false }),   // b010 
                new BitArray(new[] { false, false, true }),   // b100            
            };

            var fac = new Facette("Facetto", new List<object> { 1, 2, 3 });
            Facette.SelectResult res = new Facette.SelectResult();

            res = fac.GetValidCombinations(1, 1);
            allCombinations = allCombinations.Where(x => !res.valueCombinations.Any(y => y.SameAs(x))).ToList();                             

            Assert.AreEqual(0, allCombinations.Count);
        }

        [Test]
        public void Facette_GetValidCombinations_SelectsAllBits_TwoBits()
        {

            List<BitArray> allCombinations = new List<BitArray>
            { 
                new BitArray(new[] { true, true, false }),   // b011
                new BitArray(new[] { true, false, true }),   // b101
                new BitArray(new[] { false, true, true }),   // b110              
            };

            var fac = new Facette("Facetto", new List<object> { 1, 2, 3 });
            Facette.SelectResult res = new Facette.SelectResult();
            res = fac.GetValidCombinations( 2, 2);
            allCombinations = allCombinations.Where(x => !res.valueCombinations.Any(y => y.SameAs(x))).ToList();

            Assert.AreEqual(0, allCombinations.Count);
            }

        [Test]
        public void Facette_GetValidCombinations_SelectsAllBits_OneAndTwoBits()
        {
            List<BitArray> allCombinations = new List<BitArray>
            {
                new BitArray(new[] { true, false, false }),   // b001
                new BitArray(new[] { false, true, false }),   // b010 
                new BitArray(new[] { true, true, false }),   // b011
                new BitArray(new[] { false, false, true }),   // b100
                new BitArray(new[] { true, false, true }),   // b101
                new BitArray(new[] { false, true, true }),   // b110              
            };

            var fac = new Facette("Facetto", new List<object> { 1, 2, 3 });
            Facette.SelectResult res = new Facette.SelectResult();
            res = fac.GetValidCombinations( 1, 2);
            allCombinations = allCombinations.Where(x => !res.valueCombinations.Any(y => y.SameAs(x))).ToList();

            Assert.AreEqual(0, allCombinations.Count);
        }

        [Test]
        public void Facette_GetValidCombinations_SelectsAllBits_TwoBitsFourStates()
        {

            List<BitArray> allCombinations = new List<BitArray>
            {
                new BitArray(new[] { true, true, false, false }),   // b0011
                new BitArray(new[] { true, false, true, false }),   // b0101
                new BitArray(new[] { false, true, true, false }),   // b0110
                new BitArray(new[] { true, false, false, true }),   // b1001
                new BitArray(new[] { false, true, false, true }),   // b1010
                new BitArray(new[] { false, false, true, true }),   // b1100              
            };

            var fac = new Facette("Facetto", new List<object> { 1, 2, 3, 4 });
            Facette.SelectResult res = new Facette.SelectResult();

            res = fac.GetValidCombinations(2, 2);
            allCombinations = allCombinations.Where(x => !res.valueCombinations.Any(y => y.SameAs(x))).ToList();

            Assert.AreEqual(0, allCombinations.Count);
        }

        [Test]
        public void Facette_GetValues_ReturnsCorrectValues()
        {
            var fac = new Facette("Facetto", new List<object> { 1, 2, 3, 4, 5 });
            Assert.IsTrue(fac.GetValues(new BitArray(new[] { true, false, true, false, false })).SequenceEqual(new List<object> { 1, 3 }));
            Assert.IsTrue(fac.GetValues(new BitArray(new[] { false, true, false, true, false })).SequenceEqual(new List<object> { 2, 4 }));
        }
    }
}
