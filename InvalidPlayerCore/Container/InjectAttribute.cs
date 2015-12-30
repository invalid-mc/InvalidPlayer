using System;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }

        public InjectAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}