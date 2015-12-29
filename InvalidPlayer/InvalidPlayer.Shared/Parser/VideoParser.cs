using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Service;

namespace InvalidPlayer.Parser
{
    [Singleton]
    public class VideoParser : IVideoParserDispatcher
    {
        private static readonly Regex NameRegex = new Regex("://[^.]*?.(\\w+).(com|tv)");
        private readonly Dictionary<string, IVideoParser> _parsers;

        [Inject] private List<IVideoParser> _parserList;

        public VideoParser()
        {
            _parsers = new Dictionary<string, IVideoParser>(10);
        }

        public IVideoParser GetParser(string url)
        {
            var name = GetName(url).ToLower();
            if (_parsers.ContainsKey(name))
            {
                return _parsers[name];
            }
            return null;
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var parser = GetParser(url.ToLower());
            AssertUtil.NotNull(parser, "unsupport url");

            return await parser.ParseAsync(url);
        }


        private string GetName(string url)
        {
            var match = NameRegex.Match(url);
            return match.Groups[1].ToString();
        }

        [Init]
        private void InitParser()
        {
            foreach (var parser in _parserList)
            {
                _parsers.Add(parser.Name, parser);
            }
        }
    }
}