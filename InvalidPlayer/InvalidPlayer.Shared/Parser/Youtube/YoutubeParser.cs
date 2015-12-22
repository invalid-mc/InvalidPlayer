using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using InvalidPlayer.Model;

namespace InvalidPlayer.Parser.Youtube
{
   public class YoutubeParser :IVideoParser
   {
       public static readonly string Api = @"http://www.youtube.com/get_video_info?video_id={0}";
        private static readonly Regex VideoIdRegex = new Regex("v=([^&]+)");                                                       
       private readonly HttpClient _httpClient;

       public YoutubeParser()
       {
            _httpClient=new HttpClient();
       }
       public async Task<List<VideoItem>> ParseAsync(string url)
       {
           
           var id = GetVideoId(url);
           var query = string.Format(Api, id);
           var result =await _httpClient.GetStringAsync(new Uri(query));
            WwwFormUrlDecoder urlDecoder=new WwwFormUrlDecoder(result);
           return null;

       }

        public string GetVideoId(string url)
        {
            var match = VideoIdRegex.Match(url);
            return match.ToString();
        }
    }
}
