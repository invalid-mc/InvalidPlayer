using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using InvalidPlayer.Model;
using InvalidPlayer.Service;
using Newtonsoft.Json;

namespace InvalidPlayer.Parser.HunanTv
{
    public class HunanTvParser : IVideoParser
    {
        private static readonly string CustomHeaderName = "GetContentFeatures.DLNA.ORG";
        private static readonly string UserAgent = "NSPlayer/12.00.10011.16384 WMFSDK/12.00.10011.16384";
        private static readonly string ApiUrl =
            "http://win.api.hunantv.com/v2/video/getSource/?vid={0}&appVersion=1.0.0.0";

        private readonly HttpClient _httpClient;

        public string Name
        {
            get { return "hunantv"; }
        }

        public HunanTvParser()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var sourceUrl = await GetVideoSourceUrlAsync(GetVideoId(url));
            return await ParseVideoInfoAsync(sourceUrl);
        }

        private string GetVideoId(string url)
        {
            var startIndex = url.LastIndexOf('/');
            return url.Substring(startIndex + 1, url.LastIndexOf('.') - startIndex - 1);
        }

        private async Task<string> GetVideoSourceUrlAsync(string id, int definition = -1)
        {
            AssertUtil.HasText(id, "unsupport url");
            var infoUrl = string.Format(ApiUrl, id);

            var result = await _httpClient.GetAsync(new Uri(infoUrl));
            result.EnsureSuccessStatusCode();
            var rawInfo = await result.Content.ReadAsStringAsync();

            var videoInfo = JsonConvert.DeserializeObject<VideoInfo>(rawInfo);
            var videoSources = videoInfo.Data.VideoSources.OrderByDescending(s => s.Definition).ToList();
            return (videoSources.FirstOrDefault(s => s.Definition == definition) ?? videoSources[0]).Url;
        }

        private async Task<string> GetRealUrlAsync(string originUrl)
        {
            var routeResult = await _httpClient.GetAsync(new Uri(originUrl));
            routeResult.EnsureSuccessStatusCode();
            var rawRouteInfo = await routeResult.Content.ReadAsStringAsync();

            var routeInfo = JsonConvert.DeserializeObject<RouteInfo>(rawRouteInfo);
            return routeInfo.Info;
        }

        private async Task<string> GetVideoHeaderAsync(string realUrl)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(realUrl));
            message.Headers.Clear();
            message.Headers.Accept.TryParseAdd("*/*");
            message.Headers.UserAgent.TryParseAdd(UserAgent);
            message.Headers.Append(CustomHeaderName, "1");
            message.Headers.AcceptEncoding.TryParseAdd("gzip, deflate");

            var videoResult = await _httpClient.SendRequestAsync(message, HttpCompletionOption.ResponseContentRead);
            videoResult.EnsureSuccessStatusCode();

            return await videoResult.Content.ReadAsStringAsync();
        }

        private async Task<List<VideoItem>> ParseVideoInfoAsync(string url)
        {
            var realUrl = await GetRealUrlAsync(url);
            var prefixUrl = realUrl.Substring(0, realUrl.IndexOf(".mp4", StringComparison.Ordinal) + 5);

            var videoHeader = await GetVideoHeaderAsync(realUrl);
            AssertUtil.HasText(videoHeader, "cannot get video info");

            var itemsRawInfo = videoHeader.Split(new[] { "#EXTINF:" }, StringSplitOptions.RemoveEmptyEntries);
            return itemsRawInfo.Skip(1).Select(r =>
            {
                var strs = r.Split(',');
                // TODO: Size
                return new VideoItem { Seconds = double.Parse(strs[0]), Url = prefixUrl + strs[1] };
            }).ToList();
        }
    }

    /// <summary>
    /// 视频资源信息，通过VideoID产生的URL获取
    /// </summary>
    public class VideoInfo
    {
        /// <summary>
        /// 视频资源播放信息
        /// </summary>
        public class VideoSource
        {
            /// <summary>
            /// 清晰度标识：3~0，3对应下述超清，依次类推
            /// </summary>
            [JsonProperty("definition")]
            public int Definition { get; set; }

            /// <summary>
            /// 清晰度描述：超清、高清、标清、流畅
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }
        
        public class DownloadSource
        {
            [JsonProperty("definition")]
            public int Definition { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class Descriptions
        {
            [JsonProperty("lastseries")]
            public string Lastseries { get; set; }

            [JsonProperty("types")]
            public string Types { get; set; }

            [JsonProperty("area")]
            public string Area { get; set; }

            [JsonProperty("director")]
            public string Director { get; set; }

            [JsonProperty("player")]
            public string Player { get; set; }

            [JsonProperty("abscontent")]
            public string Abscontent { get; set; }
        }

        public class Info
        {
            [JsonProperty("videoId")]
            public string VideoId { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("imgUrl")]
            public string ImgUrl { get; set; }

            /// <summary>
            /// 视频信息集合，包含了不同的清晰度视频
            /// </summary>
            [JsonProperty("videoSources")]
            public VideoSource[] VideoSources { get; set; }

            [JsonProperty("downloadSources")]
            public DownloadSource[] DownloadSources { get; set; }

            [JsonProperty("relatedVideos")]
            public object[] RelatedVideos { get; set; }

            [JsonProperty("descriptions")]
            public Descriptions Descriptions { get; set; }

            [JsonProperty("preVideoId")]
            public string PreVideoId { get; set; }

            [JsonProperty("nextVideoId")]
            public string NextVideoId { get; set; }

            [JsonProperty("isfull")]
            public string Isfull { get; set; }

            [JsonProperty("collectionId")]
            public string CollectionId { get; set; }

            [JsonProperty("cid")]
            public string Cid { get; set; }

            [JsonProperty("pay")]
            public string Pay { get; set; }
        }

        [JsonProperty("err_code")]
        public int ErrCode { get; set; }

        [JsonProperty("err_msg")]
        public string ErrMsg { get; set; }

        [JsonProperty("data")]
        public Info Data { get; set; }
    }

    /// <summary>
    /// 视频资源网址转换之后的信息，包含了真实地址
    /// </summary>
    public class RouteInfo
    {
        [JsonProperty("ver")]
        public string Ver { get; set; }

        [JsonProperty("isothercdn")]
        public string Isothercdn { get; set; }

        /// <summary>
        /// 保存了视频真实网址
        /// </summary>
        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("loc")]
        public string Loc { get; set; }

        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("idc")]
        public string Idc { get; set; }
    }
}
