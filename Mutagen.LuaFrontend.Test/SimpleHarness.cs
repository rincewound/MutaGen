using MutagenRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend.Test
{
    public class SimpleHarness : ITestHarness
    {
        public string lastPrint;
        public void HarnessPrint(string data)
        {
            lastPrint = data;
        }

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
}
