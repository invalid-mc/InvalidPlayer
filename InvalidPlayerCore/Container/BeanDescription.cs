using System;
using System.Collections.Generic;
using System.Reflection;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    public class BeanDescription
    {
        public string Name { get; set; }
        public Type BeanType { get; set; }
        public bool IsSingleton { get; set; }
        public bool IsPrototype { get; set; }
        public bool IsLazyInit { get; set; }
        public List<string> Depends { get; set; }
        public MethodInfo Init { get; set; }
    }
}