using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InvalidPlayer.Model;
using InvalidPlayer.Parser.Sohu;
using InvalidPlayer.Parser.Tudou;
using InvalidPlayer.Parser.Youku;

namespace InvalidPlayer.Parser
{
    public class VideoParser : IVideoParser
    {
        private readonly IVideoParser _sohu;
        private readonly IVideoParser _youku;
        private readonly IVideoParser _tudou;

        public VideoParser()
        {
            //TODO 自动初始化
            //InitParser();

            _youku = new YoukuParser();
            _sohu = new SohuParser();
            _tudou=new TudouParser();

            InitParser();
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var parser = GetParser(url);

            if (null == parser)
            {
                return null;
            }
            return await parser.ParseAsync(url);
        }

        private IVideoParser GetParser(string webUrl)
        {
            if (webUrl.Contains("sohu.com"))
            {
                return _sohu;
            }

            if (webUrl.Contains("youku.com"))
            {
                return _youku;
            }

            if (webUrl.Contains("tudou.com"))
            {
                return _tudou;
            }

            return null;
        }


        private void InitParser()
        {
            //TODO 自动初始化

            var parsers = GetType().GetTypeInfo().Assembly.DefinedTypes.Where(item => typeof (IVideoParser).GetTypeInfo().IsAssignableFrom(item));
            foreach (var parser in parsers)
            {
                Debug.WriteLine(parser);
            }
        }
    }
}