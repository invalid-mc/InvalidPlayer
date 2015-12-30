using System;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    public interface IBeanFactory
    {
        bool ContainsBean(string name);
        bool ContainsBean(Type type);
        T GetBean<T>();
        T GetBean<T>(string name);
        object GetBean(string name);
        Type GetType(string name);
        bool IsPrototype(string name);
        bool IsSingleton(string name);
    }
}