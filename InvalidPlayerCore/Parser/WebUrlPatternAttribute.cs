using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvalidPlayerCore.Parser
{
    /// <summary>
    /// URL匹配正则
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class WebUrlPatternAttribute : Attribute
    {
        public WebUrlPatternAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get;private set; }
    }
}
