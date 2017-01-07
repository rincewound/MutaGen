using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    public class AssertDelegate
    {
        public delegate bool AssertFunc();

        /// <summary>
        /// Creates an assert, whose execution is bound to the result of another
        /// function (which incidently is an assert as well!), so we can do something
        /// like this:
        /// if (FacetteX.CurrentValue.Contains("Foo")
        ///     DoOtherAssert()
        /// by using GuardedAssert as:
        ///     var ass = AssertDelegate.GuardedAssert(DoOtherAssert, () => FacetteX.CurrentValue.Contains("Foo"));
        /// if the Guard is not triggered, this is always returns true (i.e. the Assertion never fails), 
        /// otherwise it returns the value of the guarded call.
        /// </summary>
        /// <param name="funcToGuard"></param>
        /// <param name="Guard"></param>
        /// <returns></returns>
        public static AssertFunc GuardedAssert(AssertFunc funcToGuard, AssertFunc Guard)
        {
            return () => { if (Guard()) { return funcToGuard(); } return true; };
        }
    }
}
