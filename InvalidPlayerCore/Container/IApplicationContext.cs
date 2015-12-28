using System.Collections.Generic;
//
//YUKI FOREVER
//
// 
namespace InvalidPlayerCore.Container
{
    public interface IApplicationContext
    {
         Dictionary<string, T> GetBeansOfType<T>();

         string getId();
    }
}
