using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutagenRuntime
{
    public static class BitArrayHelper
    {
        public static bool SameAs(this BitArray a, BitArray b)
        {
            if (a.Length != b.Length)
                return false;
            var max = a.Length > b.Length ? b.Length : a.Length;
            for (int i = 0; i < max; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
    }
}
