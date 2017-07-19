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
            public Facette theFacette;
            public BitArray valueSet;
            public Binding next { get;  set; }
            public Binding prev { get;  set; }

            public Binding() { }
            
            public Binding Clone()
            {
                Binding b = new Binding();
                b.theFacette = theFacette;
                b.valueSet = valueSet;
                b.prev = prev;
                if(next != null)
                    b.next = next.Clone();
                return b;
            }
            

            public Binding Head()
            {
                if (prev != null)
                    return prev.Head();
                return this;
            }

            // Should yield true, if the binding cannot fulfill the constraint
            // c.
            public bool ViolatesConstraint(Constraint c)
            {
                if(!c.IsActive(theFacette, valueSet))
                {
                    // the constraint does not apply to this concrete binding. Ask
                    // next in list.
                    if (next != null)
                        return next.ViolatesConstraint(c);

                    // we're at the end of the chain and nobody bothered yet - 
                    // so we apparently don't violate the constraint.
                    return false;
                }

                // constraint is active, check if the constrained facette's valueset
                // fullfills the constraint.
                var head = Head();

                return !head.IsValuesetLegal(c);                    
            }

            /// <summary>
            /// Yields TRUE, if the valueset of this binding
            /// and the matching facette is legel with resprect to
            /// the given constraint.
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            private bool IsValuesetLegal(Constraint c)
            {
                var valuesetIsLegal = c.ValueSetFullfillsConstraint(theFacette, theFacette.GetValues(valueSet));

                if (!valuesetIsLegal)
                    return false;

                if (next == null)
                    return valuesetIsLegal;

                return next.IsValuesetLegal(c);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine(theFacette.Name + " : " + theFacette.GetValues(valueSet).EntriesToString());

                if (next != null)
                    sb.AppendLine(next.ToString());

                return sb.ToString();
            }
        }

        public virtual void AddFacette(Facette fac)
        {
            logger.Info("Added Facette " + fac + " to test environemnt");
            allFacettes.Add(fac.Name, fac);
        }

        public virtual void AddConstraint(Constraint c)
        {
            allConstraints.Add(c);
        }

        public Facette GetFacette(string facetteName)
        {
            Facette fac;
            if (!allFacettes.TryGetValue(facetteName, out fac))
                throw new NoSuchFacetteException();

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
                binding.valueSet = v;
                binding.theFacette = head.owner;
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

                    // Attention -> We might have an issue with clones of bindings here!
                    var newBinding = myB.Clone();
                    var tailB = tb.Clone();
                    newBinding.next = tailB;
                    tailB.prev = newBinding;

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
