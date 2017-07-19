using MicroIOC;
using Mutagen.LuaFrontend;
using MutagenRuntime;
using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaRunner
{

    class Program
    {
        static void Write(string line)
        {
            System.Console.WriteLine(line);
        }

        static int Main(string[] args)
        {
            // Setup IOC Container
            IOC.Register<IApiBridge>(      () => new DefaultApiBridge());
            IOC.Register<ITestEnvironment>(() => new TestEnvironment());
            IOC.Register<ITestContext>(    () => new TestContext());
            IOC.Register<Lua>(             () => new Lua());
            IOC.Register<ApiAdapter>(      () => new ApiAdapter());

            Write("Mutagen Lua Runner");
            Write("Version " + Assembly.GetExecutingAssembly().GetName().Version);

            foreach(var str in args)
            {
                ScriptRunner runner = new ScriptRunner();
                runner.Load(new System.IO.FileStream(str, FileMode.Open));
                var theResult = runner.Run();
            }

            return -1;
        }
    }
}
