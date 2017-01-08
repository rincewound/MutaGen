using MicroIOC;
using NLog;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [MuImport]
        static ITestEnvironment te;

        [MuImport]
        static ITestContext testContext;

        static IAssertable testCaseCode;
        static ITestHarness theHarness;

        public static void Init()
        {
            te = IOC.Resolve<ITestEnvironment>();
            testContext = IOC.Resolve<ITestContext>();
            testCaseCode = null;
            theHarness = null;

            logger.Info("MutaGen Api - Init");
        }

        public static void CreateFacette(string facetteName, List<object> facetteValues)
        {
            // Should create a new facette in the global scope
            te.AddFacette(new Facette(facetteName, facetteValues));

            logger.Info("Add facette: " + facetteName);
            logger.Info("Values: " + facetteValues.EntriesToString());
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
            logger.Info("Load harness " + usedHarness + " from " + assemblyName);
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
            {
                var ex = new InvalidCastException("The type " + usedHarness + " is not a testharness.");
                logger.ErrorException("Not a harness!", ex);
                throw ex;
            }

            // Instanciate Testharness, ready to go :)
            theHarness = harness.GetConstructor(new Type[0]).Invoke(new object[0]) as ITestHarness;
            testContext = IOC.Resolve<ITestContext>();
            testCaseCode = null;
        }


        /// <summary>
        /// Adds an existing facette to the current testcase
        /// </summary>
        /// <param name="facetteName">Name of the facette to be added</param>
        /// <param name="minValues">Minimum number of values to apply each run (i.e. 2 - creates sets of at least 2 values that are passed to the apply facette function)</param>
        /// <param name="maxValues"></param>
        public static void AddFacette(string facetteName, int minValues, int maxValues)
        {
            if (theHarness == null)
            {
                var ex = new NoTestCaseBegunException();
                logger.ErrorException("Called AddFacette without beginnng a testcase!", ex);
                throw ex;
            }

            testContext.AddFacette(facetteName, minValues, maxValues);
        }

        public static void CommitTestCaseCode(IAssertable code)
        {
            testCaseCode = code;
        }

        public static void ExecTestCase()
        {
            if (testCaseCode == null)
            {
                var ex = new NoTestCaseException();
                logger.ErrorException("Called ExecTestCase without specifying a testcase (callCommitTestCaseCode first!)!", ex);
                throw ex;
            }

            if (theHarness == null)
            {
                var ex = new NoTestCaseStartedException();
                logger.ErrorException("Called ExecTestCase without specifying a testharness (call BeginTestCase first!)!", ex);
                throw ex;
            }

            var bindings = te.CreateBindings(testContext);
            logger.Info("================EXEC TEST CASE============================");
            foreach (var bnd in bindings)
            {
                
                logger.Info("=================Starting next run========================");
                ApplyBinding(bnd);
                var result = testCaseCode.Execute();
            }
            logger.Info("===============END EXEC===================================");
        }

        private static void ApplyBinding(TestEnvironment.Binding bnd)
        {
            var current = bnd;
            do
            {
                var vals = current.theFacette.GetValues(current.valueSet);
                logger.Info("Apply binding for facette " + bnd.theFacette.Name);
                logger.Info("Using values: " + vals.EntriesToString());

                var bindingSetterFunction = "Apply" + bnd.theFacette.Name;
                var theFunc = theHarness.GetType().GetMethod(bindingSetterFunction);
                theFunc.Invoke(theHarness, System.Reflection.BindingFlags.Default, null, 
                               new object[] { vals}, 
                               System.Globalization.CultureInfo.DefaultThreadCurrentCulture);

                current = current.next;
            } while (current != null);
        }

        // Dodgy naming, so that the TC code looks nicer, i.e.
        // Api.Testharness().DoStuff() instead of Api.GetTestHarness().DoStuff()
        public static ITestHarness Testharness()
        {
            return theHarness;
        }
    }
}
