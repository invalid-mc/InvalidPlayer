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
   public class BeanDescription
    {
        public string Name;
        public Type BeanType;
        public bool IsSingleton;
        public bool IsPrototype;
        public bool IsLazyInit;
        List<string> Depends;
    }
}
