using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Parser.Attributes;
using InvalidPlayerCore.Service;

namespace InvalidPlayer.Parser.Youtube
{
    [Singleton]
    [WebUrlPattern(@"https?:/*[^/]+youtube.com[^\s]+")]
    [HostNameMatch("youtube.com")]
    public class YoutubeParser : IVideoParser
    {
        public static readonly string Api = @"http://www.youtube.com/get_video_info?video_id={0}";
        private static readonly Regex VideoIdRegex = new Regex("v=([^&]+)");

        [Inject]
        private HttpClientService _httpClientService;
        
        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var id = GetVideoId(url);
            AssertUtil.HasText(id, "no id");

            var query = string.Format(Api, id);
            var result = await _httpClientService.GetStringAsync(query);
            AssertUtil.HasText(result, "no info from server");

            var form = new WwwFormUrlDecoder(result);
            var adaptiveFmts = form.GetFirstValueByName("adaptive_fmts");
            AssertUtil.HasText(result, "no videos");

            var adaptiveFmtsform = new WwwFormUrlDecoder(adaptiveFmts);

            //TODO    test only
            var dic = new Dictionary<string, string>();
            var quality = "";
            var type = "";
            foreach (var item in adaptiveFmtsform)
            {
                var key = item.Name;
                var value = item.Value;
                Debug.WriteLine(key + ":" + value);
                if ("url" == key)
                {
                    if (type.Contains("codecs=\"avc"))
                    {
                        dic.Add(quality, value);
                    }
                }
                else if ("quality_label" == key)
                {
                    quality = value;
                }
                else if ("type" == key)
                {
                    type = value;
                }
            }


            var videoUrl = "";
            if (dic.ContainsKey("1080p"))
            {
                videoUrl = dic["1080p"];
            }
            else if (dic.ContainsKey("1080p60"))
            {
                videoUrl = dic["1080p60"];
            }
            else if (dic.ContainsKey("720p60"))
            {
                videoUrl = dic["720p60"];
            }
            else if (dic.ContainsKey("720p"))
            {
                videoUrl = dic["720p"];
            }
            else if (dic.ContainsKey("480p"))
            {
                videoUrl = dic["480p"];
            }
            else if (dic.ContainsKey("240p"))
            {
                videoUrl = dic["240p"];
            }
            else if (dic.ContainsKey("144p"))
            {
                videoUrl = dic["144p"];
            }


            var videoItem = new VideoItem {Url = videoUrl};
            var items = new List<VideoItem>(1) {videoItem};
            return items;
        }

        public string GetVideoId(string url)
        {
            var match = VideoIdRegex.Match(url);
            return match.Groups[1].ToString();
        }
    }
}