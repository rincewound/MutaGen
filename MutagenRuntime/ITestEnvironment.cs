using System.Collections.Generic;

namespace MutagenRuntime
{
    public interface ITestEnvironment
    {
        void AddFacette(Facette fac);
        List<TestEnvironment.Binding> CreateBindings(ITestContext testContext);
        Facette GetFacette(string facetteName);
    }
}