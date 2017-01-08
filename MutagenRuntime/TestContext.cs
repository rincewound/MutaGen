using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    public class TestContext : ITestContext
    {


        internal List<ContextEntry> entries = new List<ContextEntry>();

        public void AddFacette(string facetteName, int limitLow, int limitHigh)
        {
            entries.Add(new ContextEntry() { facetteName = facetteName, limitLow = limitLow, limitHigh = limitHigh });
        }

        public List<ContextEntry> GetEntries()
        {
            return entries;
        }
    }
}
