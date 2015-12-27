using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
//YUKI FOREVER
//
// 
namespace InvalidPlayerCore.Container
{
    public interface IBeanFactory
    {
        bool ContainsBean(String name);
        bool ContainsBean(Type type);
        T GetBean<T>();
        T GetBean<T>(String name);
        object GetBean(String name);
        Type GetType(String name);
        bool IsPrototype(String name);
        bool IsSingleton(String name);
    }
}
