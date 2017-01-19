using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    public interface ITestHarness
    {
        // Should return a list of all facettes (local or global!) that are active
         //List<object> GetFacetteSelection();

         //void AddFacette(Facette f);

         //void AddFacetteSlice();

        // --> Creates two temporary facettes C and D that hold the selected
        // values each run. -> TBD: Naming?
        // Select [1..2] from (A,B) to (C,D)
         //void AddMultiSelect();

        //List<object> GetCurrentFacetteValue(string facetteName);
    }
}
