using System;

namespace InvalidPlayerCore.Parser.Attributes
{
    /// <summary>
    /// 按Host名称进行匹配
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class HostNameMatchAttribute : Attribute
    {
        public string HostName { get; private set; }

        public HostNameMatchAttribute(string hostName)
        {
            HostName = hostName;
        }
    }
}