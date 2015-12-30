using System;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class BeanAttribute : Attribute
    {
        public BeanAttribute()
        {
        }

        public BeanAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}