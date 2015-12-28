using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvalidPlayerCore.Model
{
    public class DetailVideoItem:VideoItem
    {
        public IEnumerable<KeyValuePair<string, string>> Header { get; set; }
    }
}
