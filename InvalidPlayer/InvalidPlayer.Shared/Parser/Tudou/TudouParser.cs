using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using InvalidPlayer.Model;
using InvalidPlayer.Parser.Youku;
using InvalidPlayer.Service;

namespace InvalidPlayer.Parser.Tudou
{
    public class TudouParser : YoukuParser, IVideoParser
    {
        private static readonly Regex VideoIdRegex = new Regex("vcode\\s?:\\s?\'(\\w+)"); //,vcode: 'XNDI4OTYwMTg0'    ,vcode: 'XMTQxOTgyMzAyNA=='

        private readonly HttpClient _httpClient; //TODO repalce

        public TudouParser()
        {
            _httpClient = new HttpClient();
        }

        public new async Task<List<VideoItem>> ParseAsync(string url)
        {
            var id = await GetVideoId(url);
            AssertUtil.HasText(id, "invalid url");
            return await DoParseAsync(id);
        }

        public new async Task<string> GetVideoId(string url)
        {
            var uri = new Uri(url);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Headers.UserAgent.Clear();
            message.Headers.UserAgent.TryParseAdd("Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 38.0) Gecko / 20100101 Firefox / 38.0");
            //message.Headers.Referer= uri;

            var respose = await _httpClient.SendRequestAsync(message, HttpCompletionOption.ResponseContentRead);
            if (!respose.IsSuccessStatusCode)
            {
                throw new Exception(respose.ToString());
            }

            string source = null;

            //直接读取会死...so
            using (var reader = new StreamReader((await respose.Content.ReadAsInputStreamAsync()).AsStreamForRead(0), Encoding.UTF8))
            {
                source = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            var match = VideoIdRegex.Match(source);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }

            return null;
        }
    }
}