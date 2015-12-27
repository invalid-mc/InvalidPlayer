using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Service;
using InvalidPlayerParser;

namespace InvalidPlayer.Parser
{
    public class VideoParser : IVideoParserDispatcher
    {
        private static readonly Regex NameRegex = new Regex("://[^.]*?.(\\w+).(com|tv)");
        private readonly Dictionary<string, IVideoParser> _parsers;
        
        public VideoParser()
        {
            _parsers = new Dictionary<string, IVideoParser>(10);
            InitParser();
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var parser = GetParser(url.ToLower());
            AssertUtil.NotNull(parser, "unsupport url");

            return await parser.ParseAsync(url);
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


        private string GetName(string url)
        {
            var match = NameRegex.Match(url);
            return match.Groups[1].ToString();
        }


        private void InitParser()
        {
            var assembly = GetType().GetTypeInfo().Assembly;

            var parserTypes = assembly.DefinedTypes.Where(item => typeof (IVideoParser).GetTypeInfo().IsAssignableFrom(item) && !item.IsInterface && item.AsType() != typeof (VideoParser));

            AddParsers(parserTypes);

            assembly = typeof (Info).GetTypeInfo().Assembly;

            parserTypes = assembly.DefinedTypes.Where(item => typeof (IVideoParser).GetTypeInfo().IsAssignableFrom(item));

            AddParsers(parserTypes);
        }

        private void AddParsers(IEnumerable<TypeInfo> parserTypes)
        {
            foreach (var type in parserTypes)
            {
                Debug.WriteLine(type);
                var instance = Activator.CreateInstance(type.AsType()) as IVideoParser;
                _parsers.Add(instance.Name, instance);
            }
        }
    }
}