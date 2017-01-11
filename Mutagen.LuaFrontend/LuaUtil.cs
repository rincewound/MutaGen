using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.LuaFrontend
{
    public static class LuaUtil
    {
        public static void PublishObjectMethods(object instance, LuaGlobalPortable luaEnvironment)
        {
            var dyn = luaEnvironment as dynamic;

            var methods = instance.GetType().GetMethods();

            foreach (var m in methods)
            {
                if (m.ReturnType == typeof(void))
                {
                    CreateAction(instance, dyn, m);
                }
                else
                {
                    CreateFunc(instance, dyn, m);
                }
            }
        }

        private static void CreateFunc(object instance, LuaGlobalPortable luaContext, System.Reflection.MethodInfo m)
        {
            // all stuff, that has a retval, needs to be wrapped into a Func.        
            var funcParamTypes = m.GetParameters().Select(p => p.ParameterType).ToList();
            funcParamTypes.Add(m.ReturnType);
            var aTy = Type.GetType("System.Func`" + (m.GetParameters().Length + 1), false, true);
            var theType = aTy.MakeGenericType(funcParamTypes.ToArray());
            var dlg = Delegate.CreateDelegate(theType, instance, m.Name);
            luaContext[m.Name] = dlg;
        }

        private static void CreateAction(object instance, LuaGlobalPortable luaContext, System.Reflection.MethodInfo m)
        {
            var paramTypes = m.GetParameters().Select(p => p.ParameterType).ToArray();
            if (m.GetParameters().Length != 0)
            {
                var aTy = Type.GetType("System.Action`" + m.GetParameters().Length, false, true);
                var theType = aTy.MakeGenericType(paramTypes);
                var dlg = Delegate.CreateDelegate(theType, instance, m.Name);
                luaContext[m.Name] = dlg;
            }
            else
            {
                // Special Case: Function without params is an Action (note the missing brackets!)
                var aTy = typeof(Action);
                var dlg = Delegate.CreateDelegate(aTy, instance, m.Name);
                luaContext[m.Name] = dlg;
            }
        }
        
    }
}
