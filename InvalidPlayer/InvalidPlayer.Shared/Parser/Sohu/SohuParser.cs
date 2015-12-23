using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using InvalidPlayer.Model;
using InvalidPlayer.Service;
using Newtonsoft.Json;

namespace InvalidPlayer.Parser.Sohu
{
    public class SohuParser : IVideoParser
    {
        public static readonly string Api = @"http://api.tv.sohu.com/v4/video/info/{0}.json?site=1&aid=9058330&plat=12&sver=3.5.1&partner=419&api_key=1820c56b9d16bbe3381766192e134811";
        private static readonly Regex VideoIdRegex = new Regex("vid\\s?=\\s?\"(\\w+)\"");
        private readonly HttpClient _httpClient;

        public SohuParser()
        {
            _httpClient = new HttpClient();
        }

        public string Name
        {
            get { return "sohu"; }
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {
            var id = await GetVideoId(url);
            AssertUtil.HasText(id, "error get video id");

            var query = string.Format(Api, id);
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(query));
            var response = await _httpClient.SendRequestAsync(request);
            AssertUtil.IsTrue(response.IsSuccessStatusCode, "can't get video info from sohu server");

            var content = await response.Content.ReadAsStringAsync();
            var info = JsonConvert.DeserializeObject<SohuVideoInfo>(content);
            AssertUtil.NotNull(info, "error format");

            var data = info.Data;
            AssertUtil.NotNull(data, "no video");

            var videos = GetVaildItemsAndBuildVideoItems(data);
            return videos;
        }

        private List<VideoItem> GetVaildItemsAndBuildVideoItems(SohuVideoData data)
        {
            string[] urls = null;
            string[] druations = null;
            string[] sizes = null;

            if (!string.IsNullOrEmpty(data.UrlOriginalMp4))
            {
                urls = data.UrlOriginalMp4.Split(',');
                druations = data.ClipsDurationOriginal.Split(',');
                sizes = data.ClipsBytesOriginal.Split(',');
            }
            else if (!string.IsNullOrEmpty(data.UrlSuperMp4))
            {
                urls = data.UrlSuperMp4.Split(',');
                druations = data.ClipsDurationSuper.Split(',');
                sizes = data.ClipsBytesSuper.Split(',');
            }
            else if (!string.IsNullOrEmpty(data.UrlHighMp4))
            {
                urls = data.UrlHighMp4.Split(',');
                druations = data.ClipsDurationHigh.Split(',');
                sizes = data.ClipsBytesHigh.Split(',');
            }
            else if (!string.IsNullOrEmpty(data.UrlNorMp4))
            {
                urls = data.UrlNorMp4.Split(',');
                druations = data.ClipsDurationNor.Split(',');
                sizes = data.ClipsBytesNor.Split(',');
            }

            List<VideoItem> items;

            if (null == urls)
            {
                items = new List<VideoItem>(0);
            }
            else
            {
                items = new List<VideoItem>(urls.Length);
                for (var i = 0; i < urls.Length; i++)
                {
                    var videoItem = new VideoItem();
                    videoItem.Seconds =Convert.ToDouble(druations[i]);
                    videoItem.Size = Convert.ToInt32(sizes[i]);
                    videoItem.Url = BuildUrl(urls[i]);
                    items.Add(videoItem);
                }
            }

            return items;
        }


        private string BuildUrl(string url)
        {
            //TODO uid
            url += "&uid=b3bffeaea825dd151e7833c6eb5680f9&pt=7&prod=app&pg=1&cv=3.5.1&qd=419";
            return url;
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


    internal class SohuVideoData
    {
        [JsonProperty("vid")]
        public int Vid { get; set; }

        [JsonProperty("fee")]
        public int Fee { get; set; }

        [JsonProperty("video_is_fee")]
        public int VideoIsFee { get; set; }

        [JsonProperty("is_download")]
        public int IsDownload { get; set; }

        [JsonProperty("ip_limit")]
        public int IpLimit { get; set; }

        [JsonProperty("video_length_type")]
        public int VideoLengthType { get; set; }

        [JsonProperty("video_type")]
        public int VideoType { get; set; }

        [JsonProperty("video_big_pic")]
        public string VideoBigPic { get; set; }

        [JsonProperty("ver_big_pic")]
        public string VerBigPic { get; set; }

        [JsonProperty("hor_big_pic")]
        public string HorBigPic { get; set; }

        [JsonProperty("ver_high_pic")]
        public string VerHighPic { get; set; }

        [JsonProperty("hor_high_pic")]
        public string HorHighPic { get; set; }

        [JsonProperty("hor_w16_pic")]
        public string HorW16Pic { get; set; }

        [JsonProperty("hor_w8_pic")]
        public string HorW8Pic { get; set; }

        [JsonProperty("tip")]
        public string Tip { get; set; }

        [JsonProperty("video_desc")]
        public string VideoDesc { get; set; }

        [JsonProperty("video_name")]
        public string VideoName { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("album_name")]
        public string AlbumName { get; set; }

        [JsonProperty("is_trailer")]
        public int IsTrailer { get; set; }

        [JsonProperty("is_titbits")]
        public int IsTitbits { get; set; }

        [JsonProperty("is_original_code")]
        public int IsOriginalCode { get; set; }

        [JsonProperty("url_html5")]
        public string UrlHtml5 { get; set; }

        [JsonProperty("cid")]
        public int Cid { get; set; }

        [JsonProperty("cate_code")]
        public string CateCode { get; set; }

        [JsonProperty("second_cate_name")]
        public string SecondCateName { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("play_count")]
        public int PlayCount { get; set; }

        [JsonProperty("director")]
        public string Director { get; set; }

        [JsonProperty("area")]
        public string Area { get; set; }

        [JsonProperty("area_id")]
        public int AreaId { get; set; }

        [JsonProperty("aid")]
        public int Aid { get; set; }

        [JsonProperty("origin_album_id")]
        public int OriginAlbumId { get; set; }

        [JsonProperty("publish_time")]
        public string PublishTime { get; set; }

        [JsonProperty("create_time")]
        public long CreateTime { get; set; }

        [JsonProperty("video_order")]
        public int VideoOrder { get; set; }

        [JsonProperty("latest_video_count")]
        public int LatestVideoCount { get; set; }

        [JsonProperty("total_video_count")]
        public int TotalVideoCount { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("file_size_mobile")]
        public int FileSizeMobile { get; set; }

        [JsonProperty("vid_nor")]
        public int VidNor { get; set; }

        [JsonProperty("url_nor")]
        public string UrlNor { get; set; }

        [JsonProperty("url_nor_mp4")]
        public string UrlNorMp4 { get; set; }

        [JsonProperty("file_size_nor")]
        public int FileSizeNor { get; set; }

        [JsonProperty("clips_bytes_nor")]
        public string ClipsBytesNor { get; set; }

        [JsonProperty("clips_duration_nor")]
        public string ClipsDurationNor { get; set; }

        [JsonProperty("vid_high")]
        public int VidHigh { get; set; }

        [JsonProperty("url_high")]
        public string UrlHigh { get; set; }

        [JsonProperty("url_high_mp4")]
        public string UrlHighMp4 { get; set; }

        [JsonProperty("file_size_high")]
        public int FileSizeHigh { get; set; }

        [JsonProperty("clips_bytes_high")]
        public string ClipsBytesHigh { get; set; }

        [JsonProperty("clips_duration_high")]
        public string ClipsDurationHigh { get; set; }

        [JsonProperty("vid_super")]
        public int VidSuper { get; set; }

        [JsonProperty("url_super")]
        public string UrlSuper { get; set; }

        [JsonProperty("url_super_mp4")]
        public string UrlSuperMp4 { get; set; }

        [JsonProperty("file_size_super")]
        public int FileSizeSuper { get; set; }

        [JsonProperty("clips_bytes_super")]
        public string ClipsBytesSuper { get; set; }

        [JsonProperty("clips_duration_super")]
        public string ClipsDurationSuper { get; set; }

        [JsonProperty("vid_original")]
        public int VidOriginal { get; set; }

        [JsonProperty("url_original")]
        public string UrlOriginal { get; set; }

        [JsonProperty("url_original_mp4")]
        public string UrlOriginalMp4 { get; set; }

        [JsonProperty("file_size_original")]
        public int FileSizeOriginal { get; set; }

        [JsonProperty("clips_bytes_original")]
        public string ClipsBytesOriginal { get; set; }

        [JsonProperty("clips_duration_original")]
        public string ClipsDurationOriginal { get; set; }

        [JsonProperty("total_duration")]
        public double TotalDuration { get; set; }

        [JsonProperty("start_time")]
        public int StartTime { get; set; }

        [JsonProperty("end_time")]
        public int EndTime { get; set; }

        [JsonProperty("site")]
        public int Site { get; set; }

        [JsonProperty("small_pic")]
        public string SmallPic { get; set; }

        [JsonProperty("ver_small_pic")]
        public string VerSmallPic { get; set; }

        [JsonProperty("tv_id")]
        public int TvId { get; set; }

        [JsonProperty("crid")]
        public int Crid { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("caption_ver")]
        public int CaptionVer { get; set; }

        [JsonProperty("has_caption")]
        public int HasCaption { get; set; }

        [JsonProperty("original_video_url")]
        public string OriginalVideoUrl { get; set; }

        [JsonProperty("update_date")]
        public long UpdateDate { get; set; }

        [JsonProperty("data_type")]
        public int DataType { get; set; }

        [JsonProperty("pay_type")]
        public int[] PayType { get; set; }

        [JsonProperty("publish_user_code")]
        public string PublishUserCode { get; set; }

        [JsonProperty("publish_user_name")]
        public string PublishUserName { get; set; }
    }

    internal class SohuVideoInfo
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("statusText")]
        public string StatusText { get; set; }

        [JsonProperty("data")]
        public SohuVideoData Data { get; set; }
    }


    //    "vid_nor":2362821,
    //    "url_nor_mp4":",,,"
    //     "file_size_nor":55133915,
    //      "clips_bytes_nor":"11168649,11099737,11139278,11099808,10628943",
    //     "clips_duration_nor":"300.002,300.002,300.002,300.002,285.0",
}