using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace rabbmq.Dbclass
{
    public class GetEntity
    {       
        public static T GetModels<T>(string NameSpace, string ClassName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type t = assembly.GetType($"{NameSpace}.{ClassName}");
            //object model = Activator.CreateInstance(t);
            return (T)Activator.CreateInstance(t);            
            //return t;

        }
        public static T GetInstance<T>(string instanceName)
        {
            var t= (T)Assembly.Load(Assembly.GetAssembly(typeof(T)).GetName().Name).CreateInstance(typeof(T).Namespace + "." + instanceName);
            return t;
        }
        public static T GetInstance<T>(string instanceName, params object[] param)
        {
            return (T)Assembly.Load(Assembly.GetAssembly(typeof(T)).GetName().Name).CreateInstance(typeof(T).Namespace + "." + instanceName, true, BindingFlags.CreateInstance, null, param, Thread.CurrentThread.CurrentCulture, null);
        }
    }
}
