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
        private static AutoApplicationContext _context;

        private static readonly object Locker = new object();

        public static T GetBean<T>()
        {
            return _context.GetBean<T>();
        }

        public static Dictionary<string, T> GetBeansOfType<T>()
        {
            return _context.GetBeansOfType<T>();
        }

        public static void Scan(List<Assembly> assemblies)
        {
            lock (Locker)
            {
                if (null == _context)
                {
                    _context = new AutoApplicationContext();
                }
                _context.Scan(assemblies);
            }
        }

        public static void Scan(Assembly assembly)
        {
            Scan(new List<Assembly>(1) {assembly});
        }


        public static void AutoInject(object ins)
        {
            _context.Inject(ins);
        }
    }
}