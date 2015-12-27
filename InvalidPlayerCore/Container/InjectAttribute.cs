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
    [AttributeUsage( AttributeTargets.Property| AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : Attribute
    {

        public string Name { get; set; }

        public InjectAttribute()
        {
            
        }

        public InjectAttribute(string name)
        {
            Name = name;
        }
    }
}
