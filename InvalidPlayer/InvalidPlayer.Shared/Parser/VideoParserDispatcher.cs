using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Parser;

namespace InvalidPlayer.Parser
{
    [Singleton("RegexVideoParserDispatcher")]
    public class VideoParserDispatcher : IVideoParserDispatcher
    {
        [Inject] private List<IVideoParser> _parserList;
        private Dictionary<Regex, IVideoParser> _parsers;

        public IVideoParser GetParser(string url)
        {
            Debug.WriteLine("weburl:{0}", url);
            foreach (var entry in _parsers)
            {
                var regex = entry.Key;
                if (regex.IsMatch(url))
                {
                    Debug.WriteLine("support url,selected parser is :{0}", entry.Value);
                    return entry.Value;
                }
            }
            return null;
        }

        [Init]
        private void InitParser()
        {
            _parsers = new Dictionary<Regex, IVideoParser>(_parserList.Count);
            foreach (var parser in _parserList)
            {
                var pattern = parser.GetType().GetTypeInfo().GetCustomAttribute<WebUrlPatternAttribute>();
                if (null != pattern)
                {
                    Debug.WriteLine("register parser:{0} {1}", pattern.Pattern, parser);
                    var regex = new Regex(pattern.Pattern);
                    _parsers.Add(regex, parser);
                }
            }
        }
    }
}