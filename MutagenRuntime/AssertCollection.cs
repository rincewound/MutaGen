using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    public class AssertCollection : IAssertable
    {
        List<AssertDelegate.AssertFunc> functions = new List<AssertDelegate.AssertFunc>();

        public List<AssertResult> Execute()
        {
            var res = new List<AssertResult>();

            foreach(var current in functions)
            {
                var result = current();
                var theResult = new AssertResult { result = result, info = current.Method.ToString() };
                res.Add(theResult);
            }
            return res;
        }

        public void Push(AssertDelegate.AssertFunc theFunction)
        {
            functions.Add(theFunction);
        }
    }
}
