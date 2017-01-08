using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    static class ListToStringHelper
    {
        public static string EntriesToString(this List<object> lst)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach(var val in lst)
            {
                sb.Append("[" + val.ToString() + "]");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
