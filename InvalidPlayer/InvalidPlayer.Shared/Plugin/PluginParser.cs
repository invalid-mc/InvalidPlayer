using System.Collections.Generic;
using System.Threading.Tasks;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Parser.Attributes;
using Newtonsoft.Json;

#pragma warning disable 169

namespace InvalidPlayer.Plugin
{
    [Singleton]
    [WebUrlPattern(@"https://github.com[^\s]+")]
    public class PluginParser : IVideoParser
    {
        [Inject]
        private IExternalInterface _externalInterface;

        private TaskCompletionSource<List<VideoItem>> _taskCompletionSource;

        public Task<List<VideoItem>> ParseAsync(string url)
        {
            _taskCompletionSource = new TaskCompletionSource<List<VideoItem>>();
            var t = _externalInterface.CallAsync("parserManager.execute", url);
            return _taskCompletionSource.Task;
        }


        [Init]
        public void Init()
        {
            
        }

        public void Callback(string url)
        {
            var result = new List<VideoItem> {new VideoItem {Url = url}};
            _taskCompletionSource.TrySetResult(result);
        }

        public void CallbackJson(string json)
        {
            var result= JsonConvert.DeserializeObject<List<VideoItem>>(json);
            _taskCompletionSource.TrySetResult(result);
        }
    }
}