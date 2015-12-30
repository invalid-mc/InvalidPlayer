using System.Collections.Generic;

namespace InvalidPlayerCore.Model
{
    public class DetailVideoItem : VideoItem
    {
        public IEnumerable<KeyValuePair<string, string>> Header { get; set; }
    }
}