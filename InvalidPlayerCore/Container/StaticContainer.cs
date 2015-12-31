using System.Collections.Generic;
using System.Reflection;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    public static class StaticContainer
    {
        /// <summary>
        /// 提供延迟初始化
        /// </summary>
        private static class Provider
        {
            public static readonly AutoApplicationContext ContextProvider = new AutoApplicationContext();
        }

        private static AutoApplicationContext Context
        {
            get { return Provider.ContextProvider; }
        }

        private static readonly object Locker = new object();

        public static object GetBean(string name)
        {
            return Context.GetBean(name);
        }

        public static T GetBean<T>()
        {
            return Context.GetBean<T>();
        }

        public static T GetBean<T>(string name)
        {
            return Context.GetBean<T>(name);
        }

        public static Dictionary<string, T> GetBeansOfType<T>()
        {
            return Context.GetBeansOfType<T>();
        }

        public static void Scan(List<Assembly> assemblies)
        {
            lock (Locker)
            {
                Context.Scan(assemblies);
            }
        }

        public static void Scan(Assembly assembly)
        {
            Scan(new List<Assembly>(1) {assembly});
        }


        public static void AutoInject(object ins)
        {
            Context.Inject(ins);
        }
    }
}