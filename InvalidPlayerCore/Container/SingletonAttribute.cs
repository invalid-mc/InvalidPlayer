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
    public class SingletonAttribute:BeanAttribute
    {
        public SingletonAttribute():base()
        {
        }

        public SingletonAttribute(string name) :base(name)
        {
           
        }
    }
}
