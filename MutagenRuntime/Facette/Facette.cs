using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    public class Facette
    {
        private readonly List<object> myValues;
        private readonly string myName;
        List<object>.Enumerator enumerator;

        public string Name { get { return myName; } }

        public class SelectResult
        {
            public Facette owner;
            public List<BitArray> valueCombinations;           
        }
        
        public Facette(string name, List<object> usedValues)
        {
            myName = name;
            myValues = usedValues;

            if (myValues == null)
                throw new ArgumentNullException("Cannot pass NULL as value parameter");

            if (myValues.Count == 0)
                throw new ArgumentException("Cannot use an empty value list");

            enumerator = usedValues.GetEnumerator();
            enumerator.MoveNext();
        }

        /*
         *  Returns each value exactly once 
         */
        public object NextValue()
        {            
            var val = enumerator.Current;
            enumerator.MoveNext();
            return val;
        }

        public SelectResult GetValidCombinations(int minEntries, int maxEntries)
        {
            
            var theResult = new SelectResult();            
            theResult.valueCombinations = AllCombinationsBitCount(myValues.Count, minEntries, maxEntries);
            theResult.owner = this;
            return theResult;
        }

        private int CountBits(BitArray ba)
        {
            int outVal = 0;
            for(int i = 0; i < ba.Length; i++)
            {
                if (ba[i])
                    outVal++;
            }
            return outVal;
        }

        private List<BitArray> AllCombinationsBitCount(int numBitsToUse, int minBits, int maxBits)
        {
            return AllCombinations(numBitsToUse, maxBits)
                        .Where(x => CountBits(x) >= minBits  && CountBits(x) <= maxBits)
                        .ToList();
        }

        private List<BitArray> AllCombinations(int numBitsToUse, int maxNumBitsToSet)
        {
            if (numBitsToUse == 1)
            {
                var retVal = new List<BitArray>();
                retVal.Add(new BitArray(myValues.Count, false));
                var oneArray = new BitArray(myValues.Count, false);
                oneArray[0] = true;
                retVal.Add(oneArray);
                return retVal;                                 
            }
            var predecessors = AllCombinations(numBitsToUse - 1, maxNumBitsToSet);

            var zeros = predecessors;
            var ones = predecessors.Select((x) =>
            {
               var baNew = new BitArray(x);
               baNew[numBitsToUse- 1] = true;
               return baNew;
            }).ToList();

            zeros.AddRange(ones);
            // prune list, i.e. remove all combinations that have too many bits set already.
            return zeros.Where(x => CountBits(x) <= maxNumBitsToSet).ToList();
        } 
        
        public List<object> GetValues(BitArray bitSet)
        {
            var retVal = new List<object>();
            for(int i = 0; i < myValues.Count; i++)
            {
                if (bitSet[i])
                {
                    retVal.Add(myValues[i]);
                }
            }
            return retVal;
        }      
    }
}
