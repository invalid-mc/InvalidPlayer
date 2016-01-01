using System.Collections.Generic;
using System.Threading.Tasks;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Parser.Attributes;
using InvalidPlayerCore.Plugin.JavascriptEngine;

#pragma warning disable 169

namespace InvalidPlayer.Plugin
{
    [Singleton]
    [WebUrlPattern(@"https://github.com[^\s]+")]
    public class PluginParser : IVideoParser
    {
        private readonly TaskCompletionSource<List<VideoItem>> _taskCompletionSource;

        [Inject]
        private WebViewEngine _webViewEngine;

        public PluginParser()
        {
            _taskCompletionSource = new TaskCompletionSource<List<VideoItem>>();
        }

        public Task<List<VideoItem>> ParseAsync(string url)
        {
            //TODO .....
            var t = _webViewEngine.ExecuteMethodAsync("parserManager.execute", url);
            return _taskCompletionSource.Task;
        }

        public void Callback(string url)
        {
            //TODO only one  for test 
            var result = new List<VideoItem> {new VideoItem {Url = url}};
            _taskCompletionSource.TrySetResult(result);
        }
    }
}