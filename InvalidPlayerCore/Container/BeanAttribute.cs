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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class BeanAttribute : Attribute
    {
        public string Name { get; set; }

        public BeanAttribute()
        {
        }

        public BeanAttribute(string name)
        {
            this.Name = name;
        }
    }
}
