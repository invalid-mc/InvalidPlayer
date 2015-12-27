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
    public class PrototypeAttribute : BeanAttribute
    {
        public PrototypeAttribute():base()
        {
        }

        public PrototypeAttribute(string name) :base(name)
        {

        }
    }
}
