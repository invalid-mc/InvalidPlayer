using System;

namespace InvalidPlayerCore.Container
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InitAttribute : Attribute
    {
    }
}