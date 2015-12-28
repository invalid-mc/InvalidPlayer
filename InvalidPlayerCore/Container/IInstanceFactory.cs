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
   public interface IInstanceFactory
    {
        object  GetInstance(Type type);
    }
}
