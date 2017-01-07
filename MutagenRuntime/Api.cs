using MicroIOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    /*
     *     +--- uses --> Runner----uses---+ 
     *     +--- creates --> Assertable <--+
     *    TC -- uses --> Harness --- uses --> TestEnvBase (accessed via API)
     *                [TestHarness Specific]     [MRuntime, Facettes, Bindings]
     */

    public static class Api
    {
        static TestEnvironment te;
        static IAssertable testCaseCode;
        static ITestHarness theHarness;

        public static void Init()
        {
           te = IOC.Resolve<TestEnvironment>();
        }

        public static void CreateFacette(string facetteName, List<object> facetteValues)
        {
            // Should create a new facette in the global scope
            te.AddFacette(new Facette(facetteName, facetteValues));
        }

        /// <summary>
        /// Instanciates the given harness and makes it accessible.
        /// Throws exceptions, if 
        ///  ...type is not found
        ///  ... type is not a harness
        ///  ... assembly hosting the harness is not found.
        /// </summary>
        /// <param name="usedHarness"></param>
        /// <param name="assemblyName"></param>
        public static void BeginTestCase(string usedHarness, string assemblyName)
        {
            Type harness;
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                var assembly = System.Reflection.Assembly.LoadFrom(assemblyName);
                harness = assembly.GetType(usedHarness);
            }
            else
            {
                // Find implementing testharness
                harness = Type.GetType(usedHarness, null, null, true);
            }

            var implementsTestharness = harness.GetInterface("ITestHarness") != null;

            if (!implementsTestharness)
                throw new InvalidCastException("The type " + usedHarness + " is not a testharness.");

            // Instanciate Testharness, ready to go :)
            theHarness = harness.GetConstructor(new Type[0]).Invoke(new object[0]) as ITestHarness;
        }

        public static void ExecTestCase()
        {
            var bindings = te.CreateBindings(null);
            foreach(var bnd in bindings)
            {
                //ApplyBinding(bnd);
                var result = testCaseCode.Execute();
            }
        }

        public static bool ApplyNextBinding()
        {
            //Generate next variable binding, yield false, if all possible bindings are exhausted.
            return false;
        }

        // Dodgy naming, so that the TC code looks nicer, i.e.
        // Api.Testharness().DoStuff() instead of Api.GetTestHarness().DoStuff()
        public static ITestHarness Testharness()
        {
            return theHarness;
        }
    }
}
