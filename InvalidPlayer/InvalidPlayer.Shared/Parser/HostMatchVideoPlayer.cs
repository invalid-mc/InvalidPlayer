using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Parser.Attributes;

#pragma warning disable 649

namespace InvalidPlayer.Parser
{
    [Singleton("HostMatchVideoPlayer")]
    public class HostMatchVideoPlayer : WebVideoPlayer
    {
        [Inject]
        private List<IVideoParser> _parserList;

        private static readonly Regex NameRegex = new Regex(@"://(?:(?:www\.)?(?:m\.)?)*([^/]+)");
        private Dictionary<string, IVideoParser> _parsers;

        private string _url;

        public override string Url
        {
            get { return _url; }
            set { SetCurrentParser(_url = value); }
        }

        private void SetCurrentParser(string url)
        {
            var match = NameRegex.Match(url);
            var name = match.Groups[1].ToString().ToLower();
            Parser = _parsers.ContainsKey(name) ? _parsers[name] : null;
        }

        //        public async Task<List<VideoItem>> ParseAsync(string url)
        //        {
        //            var parser = GetParser(url.ToLower());
        //            AssertUtil.NotNull(parser, "unsupport url");
        //            var result= await parser.ParseAsync(url);
        //            Debug.WriteLine(result);
        //            return result;
        //        }

        [Init]
        private void InitParser()
        {
            // TODO: use null condition expression
            _parsers = new Dictionary<string, IVideoParser>(_parserList.Count);
            foreach (var parser in _parserList)
            {
                var names = parser.GetType().GetTypeInfo().GetCustomAttributes<HostNameMatchAttribute>();
                foreach (var name in names)
                {
                    _parsers.Add(name.HostName, parser);
                }
            }
        }
    }
}