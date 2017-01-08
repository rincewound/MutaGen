using System.Collections.Generic;

namespace MutagenRuntime
{
    public interface ITestContext
    {
        void AddFacette(string facetteName, int limitLow, int limitHigh);
        List<ContextEntry> GetEntries();
    }
}