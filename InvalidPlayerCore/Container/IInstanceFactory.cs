using System;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    public interface IInstanceFactory
    {
        object GetInstance(Type type);
    }
}