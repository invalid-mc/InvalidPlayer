using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using InvalidPlayer.Model;
using InvalidPlayer.Service;
using Newtonsoft.Json;

namespace InvalidPlayer.Parser.Youku
{
    public class YoukuParser : IVideoParser
    {
        private static readonly Regex VideoIdRegex = new Regex("(?<=id_)(\\w+)");
        private static readonly string Did = "1da86bf9970e91bd58f0312781e761c6";
        private static readonly string VideoFormat = "1,3,5,7"; //HD, ThreeGP, FLV, ThreeGPHD, FLVHD,  M3U8,  HD2
        private static readonly string UserAgent = "Youku;3.7;WindowsPhone;8.10.14219.0;8X";
        private static readonly string ApiUrl = @"http://api.3g.youku.com/common/v3/play?pid=ad00404c17cb662e&id={0}&format={1}&point=1&audiolang=1&language=&did={2}&ctype=22&0cc1fdbf409f43a49c1ceaebfcc56e57=&guid={2}&ver=3.7&network=WIFI&os=WindowsPhone&os_ver=8.10.14219.0&brand=HTC&btype=HTC6990LVW";

        private readonly HttpClient _httpClient;

        public YoukuParser()
        {
            _httpClient = new HttpClient();
        }

        public string Name
        {
            get { return "youku"; }
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var id = GetVideoId(url);
            AssertUtil.HasText(id, "unsupport url");
            return await DoParseAsync(id);
        }

        public async Task<List<VideoItem>> DoParseAsync(string id)
        {
            var infoUrl = string.Format(ApiUrl, id, VideoFormat, Did);

            var result = await GetVideoInfoFromServer(infoUrl);
            AssertUtil.HasText(result, "can't get video info  from youku server");

            var info = JsonConvert.DeserializeObject<PlayInfo>(result);
            var decodedData = YoukuAes.Decrypt(info.Data);
            var decodedInfo = JsonConvert.DeserializeObject<DecodedPlayInfo>(decodedData);
            AssertUtil.NotNull(decodedInfo.Results, "no results");

            var items = GetVaildItmes(decodedInfo);
            AssertUtil.NotNull(items, "no videos");

            var playInfoSidData = decodedInfo.Sid_data;

            var videoItems = new List<VideoItem>(items.Length);

            if (null == playInfoSidData)
            {
                videoItems.AddRange(items.Select(item => new VideoItem {Seconds = item.Seconds, Size = item.Size, Url = item.Url}));
            }
            else
            {
                foreach (var item in items)
                {
                    var newUrl = await BuildUrlWithToken(item.Url, playInfoSidData, item.FileId, Did);
                    videoItems.Add(new VideoItem {Seconds = item.Seconds, Size = item.Size, Url = newUrl});
                }
            }

            return videoItems;
        }

        private PlayInfoResultItem[] GetVaildItmes(DecodedPlayInfo decodedInfo)
        {
            var hd2 = decodedInfo.Results.Hd2;
            if (null != hd2 && hd2.Length > 0)
            {
                Debug.WriteLine("hd2 from youku");
                return hd2;
            }

            var mp4 = decodedInfo.Results.Mp4;
            if (null != mp4 && mp4.Length > 0)
            {
                Debug.WriteLine("mp4 from youku");
                return mp4;
            }

            var flvhd = decodedInfo.Results.Flvhd;
            if (null != flvhd && flvhd.Length > 0)
            {
                Debug.WriteLine("flvhd from youku");
                return flvhd;
            }

            var flv = decodedInfo.Results.Flv;
            if (null != flv && flv.Length > 0)
            {
                Debug.WriteLine("flv from youku");
                return flv;
            }

            return null;
        }

        private async Task<string> GetVideoInfoFromServer(string infoUrl)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(infoUrl));
            message.Headers.UserAgent.Clear();
            message.Headers.UserAgent.TryParseAdd(UserAgent);

            var respose = await _httpClient.SendRequestAsync(message, HttpCompletionOption.ResponseContentRead);
            if (!respose.IsSuccessStatusCode)
            {
                throw new Exception(respose.ToString());
            }

            var result = await respose.Content.ReadAsStringAsync();
            return result;
        }


        public string GetVideoId(string url)
        {
            var match = VideoIdRegex.Match(url);
            return match.ToString();
        }

        private async Task<string> BuildUrlWithToken(string url, DecodedPlayInfo.PlayInfoSidData playInfoSidData, string fileId, string did)
        {
            var param = new Dictionary<string, string>(7);
            param.Add("sid", playInfoSidData.Sid);
            param.Add("token", playInfoSidData.Token);
            param.Add("oip", playInfoSidData.Oip);
            param.Add("ev", "1");
            param.Add("ctype", "22");
            param.Add("did", did);
            var ep = YoukuAes.Encrypt(string.Join("_", param["sid"], fileId, param["token"]));
            param.Add("ep", ep);
            var content = new HttpFormUrlEncodedContent(param);
            url += "&" + await content.ReadAsStringAsync();
            return url;
        }
    }


    [JsonObject(MemberSerialization.OptOut)]
    public class PlayInfo
    {
        public string Data { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class PlayInfoResult
    {
        public PlayInfoResultItem[] Mp4 { get; set; }

        [JsonProperty("3gphd")]
        public PlayInfoResultItem[] ThreeGpHd { get; set; }

        public PlayInfoResultItem[] Hd3 { get; set; }

        public PlayInfoResultItem[] Hd2 { get; set; }

        public PlayInfoResultItem[] Flvhd { get; set; }

        public PlayInfoResultItem[] Flv { get; set; }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class PlayInfoResultItem
    {
        public string Url { get; set; }

        public int Seconds { get; set; }

        public string FileId { get; set; }

        public int Id { get; set; }

        public int Size { get; set; }
    }

    public class DecodedPlayInfo
    {
        public string Status { get; set; }

        public string ShowId { get; set; }

        public string WebUrl { get; set; }

        public string Uid { get; set; }

        public string Siddecode { get; set; }

        public PlayInfoResult Results { get; set; }

        public int Copyright { get; set; }

        public string Title { get; set; }

        public long TotalSeconds { get; set; }

        public PlayInfoSidData Sid_data { get; set; }

        [JsonObject(MemberSerialization.OptOut)]
        public class PlayInfoSidData
        {
            public string Token { get; set; }

            public string Oip { get; set; }

            public string Sid { get; set; }
        }
    }
}