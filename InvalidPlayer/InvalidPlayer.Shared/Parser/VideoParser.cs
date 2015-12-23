using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InvalidPlayer.Model;
using InvalidPlayer.Service;

namespace InvalidPlayer.Parser
{
    public class VideoParser : IVideoParser
    {
        private static readonly Regex NameRegex = new Regex("://[^.]*?.(\\w+).(com|tv)");
        private readonly Dictionary<string, IVideoParser> _parsers;


        public VideoParser()
        {
            _parsers = new Dictionary<string, IVideoParser>(10);
            InitParser();
        }

        public string Name
        {
            get { return "video"; }
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var parser = GetParser(url);
            AssertUtil.NotNull(parser, "unsupport url");

            return await parser.ParseAsync(url);
        }

        private IVideoParser GetParser(string webUrl)
        {
            var name = GetName(webUrl);
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

            var parserTypes = assembly.DefinedTypes.Where(item =>typeof (IVideoParser).GetTypeInfo().IsAssignableFrom(item)&&!item.IsInterface&&item.AsType()!=typeof(VideoParser));
            foreach (var type in parserTypes)
            {
                Debug.WriteLine(type);
                var instance = Activator.CreateInstance(type.AsType()) as IVideoParser;
                _parsers.Add(instance.Name, instance);
            }
        }
    }
}