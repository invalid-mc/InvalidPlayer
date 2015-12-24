using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using InvalidPlayer.Model;
using Newtonsoft.Json;

namespace InvalidPlayer.Parser.HunanTv
{
    public partial class HunanTvParser
    {
        /*
         * 使用 m.api.hunantv.com 接口解析数据
         */

        // 可获取mp4文件以及m3u8格式文件
        private static readonly string MobileApiUrl = "http://m.api.hunantv.com/video/getbyid?videoId={0}";

        private async Task<string> GetMobileVideoSourceUrlAsync(string id)
        {
            var infoUrl = new Uri(string.Format(MobileApiUrl, id));

            var response = await _httpClient.GetAsync(infoUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            var rawInfo = await response.Content.ReadAsStringAsync();

            var videoInfo = JsonConvert.DeserializeObject<MobileVideoInfo>(rawInfo);
            return videoInfo.Data.Mp4Url.OrderBy(u => u.Substring(u.LastIndexOf('_') + 1)).Last();
        }

        private async Task<List<VideoItem>> ParseMobileVideoInfoAsync(string originalUrl)
        {
            var response = await _httpClient.GetAsync(new Uri(originalUrl));
            response.EnsureSuccessStatusCode();
            var rawInfo = await response.Content.ReadAsStringAsync();

            var routeInfo = JsonConvert.DeserializeObject<MobileRouteInfo>(rawInfo);
            return new List<VideoItem> { new VideoItem { Url = routeInfo.Info } };
        }
    }

    public class MobileVideoInfo
    {
        public class Detail
        {
            [JsonProperty("pcUrl")]
            public string PcUrl { get; set; }

            [JsonProperty("videoId")]
            public int VideoId { get; set; }

            [JsonProperty("collectionId")]
            public int CollectionId { get; set; }

            [JsonProperty("collectionName")]
            public string CollectionName { get; set; }

            [JsonProperty("lastseries")]
            public string Lastseries { get; set; }

            [JsonProperty("totalvideocount")]
            public string Totalvideocount { get; set; }

            [JsonProperty("isend")]
            public string Isend { get; set; }

            [JsonProperty("director")]
            public string Director { get; set; }

            [JsonProperty("player")]
            public string Player { get; set; }

            [JsonProperty("typeName")]
            public string TypeName { get; set; }

            [JsonProperty("rootid")]
            public string Rootid { get; set; }

            [JsonProperty("image")]
            public string Image { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("v_desc")]
            public string VDesc { get; set; }

            [JsonProperty("typeId")]
            public int TypeId { get; set; }

            [JsonProperty("publishTime")]
            public string PublishTime { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("playCount")]
            public string PlayCount { get; set; }

            [JsonProperty("like")]
            public int Like { get; set; }

            [JsonProperty("time")]
            public int Time { get; set; }

            [JsonProperty("isFull")]
            public int IsFull { get; set; }

            [JsonProperty("favorite")]
            public int Favorite { get; set; }

            [JsonProperty("isvip")]
            public int Isvip { get; set; }

            [JsonProperty("ispay")]
            public int Ispay { get; set; }

            [JsonProperty("year")]
            public int Year { get; set; }

            [JsonProperty("clip_type")]
            public int ClipType { get; set; }

            [JsonProperty("keywords")]
            public string Keywords { get; set; }

            [JsonProperty("seriescount")]
            public int Seriescount { get; set; }
        }

        /// <summary>
        /// 视频信息，URL等
        /// </summary>
        public class Info
        {
            [JsonProperty("m3u8Url")]
            public string[] M3U8Url { get; set; }

            [JsonProperty("mp4Url")]
            public string[] Mp4Url { get; set; }

            [JsonProperty("detail")]
            public Detail Detail { get; set; }
        }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public Info Data { get; set; }
    }

    public class MobileRouteInfo
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isothercdn")]
        public string Isothercdn { get; set; }

        /// <summary>
        /// Mp4文件地址
        /// </summary>
        [JsonProperty("info")]
        public string Info { get; set; }
    }
}
