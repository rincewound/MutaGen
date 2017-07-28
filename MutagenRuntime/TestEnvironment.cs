using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    /*
        The TestEnvironment contains the global scope
        of all defined testcases. 
    */

   public class TestEnvironment : ITestEnvironment
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        Dictionary<string, Facette> allFacettes = new Dictionary<string, Facette>();
        List<Constraint> allConstraints = new List<Constraint>();

        /// <summary>
        /// A Binding is a concrete value set used for a given
        /// facette, i.e. it stores, how a facette should be configured
        /// during a testrun.
        /// 
        /// The complete configuration of the environment consists of several
        /// lists of bindings.
        /// </summary>
        public class Binding
        {
            public List<KeyValuePair<Facette, BitArray>> allBindings = new List<KeyValuePair<Facette, BitArray>>();

            public Binding() { }

            public void PushValueset(Facette f, BitArray v)
            {
                allBindings.Add(new KeyValuePair<Facette, BitArray>(f, v));
            }
            
            public Binding Clone()
            {
                var bnd = new Binding();
                bnd.allBindings.AddRange(allBindings);
                return bnd;
            }

            // Should yield true, if the binding cannot fulfill the constraint,
            // and should be dropped.
            public bool ViolatesConstraint(Constraint c)
            {
                bool isActive = false;
                foreach(var b in allBindings)
                {
                    if(c.IsActive(b.Key, b.Value))
                    {
                        isActive = true;
                        break;
                    }
                }

                if (!isActive)
                    return false;

                var constrainedEntry = allBindings.FirstOrDefault(x => c.ConstrainsFacette(x.Key));

                if (constrainedEntry.Key == null)
                    return false;

                return !c.ValueSetFullfillsConstraint(constrainedEntry.Key, constrainedEntry.Key.GetValues(constrainedEntry.Value));             
            }           

            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach(var f in allBindings)
                {
                    sb.AppendLine(f.Key.Name + " : " + f.Key.GetValues(f.Value).EntriesToString());
                }
                return sb.ToString();
            }

            internal void PushRange(Binding tailB)
            {
                allBindings.AddRange(tailB.allBindings);
            }
        }

        public virtual void AddFacette(Facette fac)
        {
            logger.Info("Added Facette " + fac + " to test environemnt");
            allFacettes.Add(fac.Name, fac);
        }

        public virtual void AddConstraint(Constraint c)
        {
            if (!allFacettes.ContainsKey(c.constraintSource.Name))
                throw new NoConstraintGuardException();

            if (!allFacettes.ContainsKey(c.constraintTarget.Name))
                throw new NoConstraintGuardException();


            allConstraints.Add(c);
        }

        public Facette GetFacette(string facetteName)
        {
            Facette fac;
            if (!allFacettes.TryGetValue(facetteName, out fac))
                throw new NoConstraintGuardException();

            return fac;
        }

        public List<Binding> CreateBindings(ITestContext testContext)
        {
            var theValues = CollectFacetteValues(testContext);
            var theBindings = CreateBindingsImpl(theValues);
            return theBindings;
        }

        private List<Binding> CreateBindingsImpl(List<Facette.SelectResult> usedFacettes)
        {
            if (usedFacettes.Count == 0)
                return new List<Binding>();

            var head = usedFacettes[0];
            var vals = head.valueCombinations;
            var myBindings = new List<Binding>();
            foreach(var v in vals)
            {
                var binding = new Binding();
                binding.PushValueset(head.owner, v);
                myBindings.Add(binding);         
            }

            if (usedFacettes.Count == 1)
                return myBindings;

            // Get Subbindings:
            // We need to apply any constraints here
            List<Binding> result = new List<Binding>();
            var tailBindings = CreateBindingsImpl(usedFacettes.Skip(1).ToList());
            // Possible performance issue: We check each binding against all constraints.
            foreach (var myB in myBindings)
            {                
                foreach (var tb in tailBindings)
                {
                    var newBinding = myB.Clone();
                    var tailB = tb.Clone();
                    newBinding.PushRange(tailB);

                    if (allConstraints.Any(x => newBinding.ViolatesConstraint(x)))
                        continue;

                    result.Add(newBinding);
                }
            }
            return result;
        }

        private List<Facette.SelectResult> CollectFacetteValues(ITestContext testContext)
        {
            List<Facette.SelectResult> facetteValues = new List<Facette.SelectResult>();
            logger.Info("Collecting valid values for facettes...");

            for (int i = 0; i < testContext.GetEntries().Count; i++)
            {
                var entry = testContext.GetEntries()[i];
                var fac = GetFacette(entry.facetteName);
                var allValues = fac.GetValidCombinations(entry.limitLow, entry.limitHigh);

                logger.Info("   " + fac.Name + " uses " + entry.limitLow + " to " + entry.limitHigh + " values per combination, resulting in " + allValues.valueCombinations.Count + " sets of values.");

                facetteValues.Add(allValues);
            }
            return facetteValues;
        }
    }
}
