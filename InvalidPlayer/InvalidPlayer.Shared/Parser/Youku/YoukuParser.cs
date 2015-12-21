using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace InvalidPlayer.Parser.Youku
{
    public class YoukuParser : IVideoUrlParser
    {
        private static readonly Regex VideoIdRegex = new Regex("(?<=id_)(\\w+)");
        private static readonly string Did = "1da86bf9970e91bd58f0312781e761c6";
        private static readonly string VideoFormat = "1,3,5,7"; //HD, ThreeGP, FLV, ThreeGPHD, FLVHD,  M3U8,  HD2

        private static readonly string UserAgent = "Youku;3.7;WindowsPhone;8.10.14219.0;8X";

        private static readonly string ApiUrl =
            @"http://api.3g.youku.com/common/v3/play?pid=ad00404c17cb662e&id={0}&format={1}&point=1&audiolang=1&language=&did={2}&ctype=22&0cc1fdbf409f43a49c1ceaebfcc56e57=&guid={2}&ver=3.7&network=WIFI&os=WindowsPhone&os_ver=8.10.14219.0&brand=HTC&btype=HTC6990LVW";

        private readonly HttpClient _httpClient;

        public YoukuParser()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<string>> ParseAsync(string url)
        {
            var id = GetVideoId(url);
            Assert(string.IsNullOrEmpty(id), "invalid url");

            var infoUrl = string.Format(ApiUrl, id, VideoFormat, Did);
            return await DoParseAsync(infoUrl, Did);
        }


        public async Task<List<string>> DoParseAsync(string apiUrl, string did)
        {
            var result = await GetVideoInfo(apiUrl);
            Assert(string.IsNullOrEmpty(result), "get nothing from server");

            var info = JsonConvert.DeserializeObject<PlayInfo>(result);
            var decodedData = YoukuAes.Decrypt(info.Data);
            var decodedInfo = JsonConvert.DeserializeObject<DecodedPlayInfo>(decodedData);

            Assert(null == decodedInfo.Results, "no results");
            var items = GetVaildItmes(decodedInfo);
            Assert(null == items, "can't find support videos");

            var playInfoSidData = decodedInfo.Sid_data;

            var videoUrls = new List<string>(items.Length);

            if (null == playInfoSidData)
            {
                videoUrls.AddRange(items.Select(item => item.Url));
            }
            else
            {
                var param = new Dictionary<string, string>(3);
                param.Add("sid", playInfoSidData.Sid);
                param.Add("token", playInfoSidData.Token);
                param.Add("oip", playInfoSidData.Oip);
                foreach (var item in items)
                {
                    var add = BuildUrlWithToken(item.Url, param, item.FileId, did);
                    videoUrls.Add(add);
                }
            }

            return videoUrls;
        }

        private static PlayInfoResultItem[] GetVaildItmes(DecodedPlayInfo decodedInfo)
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

        private async Task<string> GetVideoInfo(string apiUrl)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(apiUrl));
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


        private string BuildUrlWithToken(string url, Dictionary<string, string> param, string fileId, string did)
        {
            var sb = new StringBuilder(url);
            sb.Append("&");
            if (param != null)
            {
                foreach (var entry in param)
                {
                    sb.Append(entry.Key).Append("=");
                    var value = entry.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        sb.Append("");
                    }
                    else
                    {
                        sb.Append(WebUtility.UrlEncode(value));
                    }
                    sb.Append('&');
                }
                sb.Append("ev=1&ctype=22&");
                sb.Append("did=").Append(WebUtility.UrlEncode(did)).Append('&');
                var ep = YoukuAes.Encrypt(param["sid"] + "_" + fileId + "_" + param["token"]);
                sb.Append("ep=").Append(WebUtility.UrlEncode(ep));
            }
            return sb.ToString();
        }

        private void Assert(bool value, string message)
        {
            if (value)
            {
                throw new Exception(message);
            }
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