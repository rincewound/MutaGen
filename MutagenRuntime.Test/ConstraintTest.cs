using MutagenRuntime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTest
{
    [TestFixture]
    class ConstraintTest
    {
        Facette F1;
        Facette F2;

        [SetUp]
        public void Setup()
        {
            F1 = new Facette("F1", new List<object> { 1, 2, 3 });
            F2 = new Facette("F2", new List<object> { 4, 5, 6 });
        }


        [Test]
        public void CanCreateSingleConstraint()
        {
            Constraint c = new Constraint(F1,F2, (x) => { return x.Any(y => (new[] { 1, 2 }).Contains((int)y)); }, new List<object> { 4 });
        }

        [Test]
        public void ConstraintTriggersForSingleValue()
        {
            Constraint c = new Constraint(F1, F2, (x) => { return x.Any(y => (new[] { 1, 2 }).Contains((int)y)); }, new List<object> { 4 });
            var val = F1.GetValidCombinations(1, 1).valueCombinations[0];
            Assert.IsTrue(c.IsActive(F1, val));
        }

    }
}
