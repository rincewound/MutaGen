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
        Dictionary<string, Facette> allFacettes = new Dictionary<string, Facette>();

        public class Binding
        {
            public Facette theFacette;
            public BitArray valueSet;
            public Binding next;

            public Binding() { }
            public Binding(Binding other, Binding next)
            {
                theFacette = other.theFacette;
                valueSet = other.valueSet;
                this.next = next;
            }

            public Binding Clone()
            {
                Binding b = new Binding();
                b.theFacette = theFacette;
                b.valueSet = valueSet;
                if(next != null)
                    b.next = next.Clone();
                return b;
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
            allFacettes.Add(fac.Name, fac);
        }

        public Facette GetFacette(string facetteName)
        {
            return allFacettes[facetteName];
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
            List<Binding> result = new List<Binding>();
            var tailBindings = CreateBindingsImpl(usedFacettes.Skip(1).ToList());
            foreach(var myB in myBindings)
            {
                foreach(var tb in tailBindings)
                {
                    var newBinding = myB.Clone();
                    newBinding.next = tb.Clone();
                    result.Add(newBinding);
                }
            }
            return result;
        }

        private List<Facette.SelectResult> CollectFacetteValues(ITestContext testContext)
        {
            List<Facette.SelectResult> facetteValues = new List<Facette.SelectResult>();

            for (int i = 0; i < testContext.GetEntries().Count; i++)
            {
                var entry = testContext.GetEntries()[i];
                var fac = GetFacette(entry.facetteName);
                var allValues = fac.GetValidCombinations(entry.limitLow, entry.limitHigh);
                facetteValues.Add(allValues);
            }
            return facetteValues;
        }
    }
}
