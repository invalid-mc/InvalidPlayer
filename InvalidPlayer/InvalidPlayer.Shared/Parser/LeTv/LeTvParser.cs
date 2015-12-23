using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using InvalidPlayer.Model;
using InvalidPlayer.Service;
using Newtonsoft.Json;

namespace InvalidPlayer.Parser.LeTv
{
    public class LeTvParser : IVideoParser
    {
        private static readonly Regex VideoIdRegex = new Regex("mmsid\\s?:\\s?(\\d+)"); //mmsid:32999296
        private static readonly string ApiUrl = @"http://dynamic.meizi.app.m.letv.com/android/dynamic.php?mod=minfo&ctl=videofile&act=index&mmsid={0}&playid=0&tss=no&pcode=010610000&version=1.9";

        private readonly HttpClient _httpClient;

        public LeTvParser()
        {
            _httpClient = new HttpClient();
        }

        public string Name
        {
            get { return "letv"; }
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var id = await GetVideoId(url);
            var queryUrl = string.Format(ApiUrl, id);
            var result = await _httpClient.GetStringAsync(new Uri(queryUrl));
            AssertUtil.HasText(result, "get nothing form letv server");

            var info = JsonConvert.DeserializeObject<LeTvResponse>(result);
            AssertUtil.NotNull(info, "format error");

            var mp4 = SelectVideo(info);
            AssertUtil.NotNull(mp4, "no invalid videos");

            var mainUrl = mp4.MainUrl;
            var videoInfoResult = await _httpClient.GetStringAsync(new Uri(mainUrl));
            AssertUtil.NotNull(mp4, "no invalid videos");

            var videoInfo = JsonConvert.DeserializeObject<VideoInfo>(videoInfoResult);
            var videoUrl = videoInfo.Location;
            AssertUtil.NotNull(mp4, "no invalid videoUrl");

            var videoItem = new VideoItem {Url = videoUrl, Seconds = videoInfo.Slicetime};
            var items = new List<VideoItem>(1) {videoItem};
            return items;
        }

        private Mp4Info SelectVideo(LeTvResponse info)
        {
            Infos infos = null;
            try
            {
                infos = info.Body.Videofile.Infos;
            }
            catch (Exception)
            {
                // 
            }

            AssertUtil.NotNull(infos, "error result");

            if (null != infos.Mp41300)
            {
                return infos.Mp41300;
            }
            if (null != infos.Mp41000)
            {
                return infos.Mp41000;
            }
            if (null != infos.Mp4350)
            {
                return infos.Mp4350;
            }
            if (null != infos.Mp4180)
            {
                return infos.Mp4180;
            }
            return null;
        }

        public async Task<string> GetVideoId(string url)
        {
            var source = await _httpClient.GetStringAsync(new Uri(url));
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }
            var match = VideoIdRegex.Match(source);
            return match.Groups[1].ToString();
        }
    }


    internal class Header
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("markid")]
        public string Markid { get; set; }
    }

    internal class Mp4Info
    {
        [JsonProperty("backUrl2")]
        public string BackUrl2 { get; set; }

        [JsonProperty("filesize")]
        public string Filesize { get; set; }

        [JsonProperty("backUrl1")]
        public string BackUrl1 { get; set; }

        [JsonProperty("mainUrl")]
        public string MainUrl { get; set; }

        [JsonProperty("backUrl0")]
        public string BackUrl0 { get; set; }

        [JsonProperty("storePath")]
        public string StorePath { get; set; }
    }


    internal class Infos
    {
        [JsonProperty("mp4_180")]
        public Mp4Info Mp4180 { get; set; }

        [JsonProperty("mp4_1300")]
        public Mp4Info Mp41300 { get; set; }

        [JsonProperty("mp4_350")]
        public Mp4Info Mp4350 { get; set; }

        [JsonProperty("mp4_1000")]
        public Mp4Info Mp41000 { get; set; }
    }

    internal class Videofile
    {
        [JsonProperty("infos")]
        public Infos Infos { get; set; }

        [JsonProperty("mmsid")]
        public string Mmsid { get; set; }
    }

    internal class Body
    {
        [JsonProperty("videofile")]
        public Videofile Videofile { get; set; }
    }

    internal class LeTvResponse
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }
    }

    internal class Nodelist
    {
        [JsonProperty("gone")]
        public int Gone { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pid")]
        public int Pid { get; set; }

        [JsonProperty("aid")]
        public int Aid { get; set; }

        [JsonProperty("isp")]
        public int Isp { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("playlevel")]
        public int Playlevel { get; set; }

        [JsonProperty("slicetime")]
        public int Slicetime { get; set; }

        [JsonProperty("leavetime")]
        public int Leavetime { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }

    internal class VideoInfo
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("ercode")]
        public int Ercode { get; set; }

        [JsonProperty("ipint")]
        public string Ipint { get; set; }

        [JsonProperty("remote")]
        public string Remote { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("ipstart")]
        public string Ipstart { get; set; }

        [JsonProperty("ipend")]
        public string Ipend { get; set; }

        [JsonProperty("geo")]
        public string Geo { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("gone")]
        public int Gone { get; set; }

        [JsonProperty("buss")]
        public string Buss { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("usep2p")]
        public int Usep2p { get; set; }

        [JsonProperty("flag")]
        public string Flag { get; set; }

        [JsonProperty("pool")]
        public string Pool { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("playlevel")]
        public int Playlevel { get; set; }

        [JsonProperty("slicetime")]
        public int Slicetime { get; set; }

        [JsonProperty("leavetime")]
        public int Leavetime { get; set; }

        [JsonProperty("expect")]
        public int Expect { get; set; }

        [JsonProperty("actual")]
        public int Actual { get; set; }

        [JsonProperty("needtest")]
        public int Needtest { get; set; }

        [JsonProperty("curtime")]
        public int Curtime { get; set; }

        [JsonProperty("starttime")]
        public int Starttime { get; set; }

        [JsonProperty("endtime")]
        public int Endtime { get; set; }

        [JsonProperty("cliptime")]
        public int Cliptime { get; set; }

        [JsonProperty("timeshift")]
        public double Timeshift { get; set; }

        [JsonProperty("dir")]
        public string Dir { get; set; }

        [JsonProperty("cdnpath")]
        public string Cdnpath { get; set; }

        [JsonProperty("livep2p")]
        public int Livep2p { get; set; }

        [JsonProperty("mustm3u8")]
        public int Mustm3u8 { get; set; }

        [JsonProperty("livesftime")]
        public int Livesftime { get; set; }

        [JsonProperty("livesfmust")]
        public int Livesfmust { get; set; }

        [JsonProperty("maxsftime")]
        public int Maxsftime { get; set; }

        [JsonProperty("maxslicesize")]
        public int Maxslicesize { get; set; }

        [JsonProperty("forcegslb")]
        public int Forcegslb { get; set; }

        [JsonProperty("updatecdn")]
        public int Updatecdn { get; set; }

        [JsonProperty("privrange")]
        public int Privrange { get; set; }

        [JsonProperty("openrange")]
        public int Openrange { get; set; }

        [JsonProperty("downpolicy")]
        public int Downpolicy { get; set; }

        [JsonProperty("systemload")]
        public string Systemload { get; set; }

        [JsonProperty("identify")]
        public string Identify { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        //  [JsonProperty("nodelist")]

        //  public Nodelist[] Nodelist { get; set; }
    }
}