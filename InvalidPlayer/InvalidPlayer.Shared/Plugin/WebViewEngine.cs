using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using InvalidPlayer.Plugin;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Exceptions;
using InvalidPlayerCore.Service;
using YukiRemoteManger.View.Help;

#pragma warning disable 649

namespace InvalidPlayerCore.Plugin.JavascriptEngine
{
    [Singleton]
    public class WebViewEngine : IExternalInterface
    {
        private readonly WebView _webView;

        private Dictionary<string, Func<string, Task<string>>> _cache;

        [Inject]
        private HttpClientService _httpClientService;

        [Inject]
        private PluginParser _pluginParser;


        public WebViewEngine()
        {
            _webView = new WebView();
            _webView.ScriptNotify += _webView_ScriptNotify;
            _webView.UnsafeContentWarningDisplaying += _webView_UnsafeContentWarningDisplaying;
            _webView.DOMContentLoaded += _webView_DOMContentLoaded;
            _webView.LongRunningScriptDetected += _webView_LongRunningScriptDetected;
            _webView.ContentLoading += _webView_ContentLoading;
            //   var url = "ms-appx-web:///Plugin/JsCore/Main.html";
            //  _webView.Source = new Uri(url);
            var web = new Web();
            var index = _webView.BuildLocalStreamUri("saki", "Main.html");
            _webView.NavigateToLocalStreamUri(index, web);
        }

        private void _webView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            Debug.WriteLine(args.Uri);
        }

        private void _webView_LongRunningScriptDetected(WebView sender, WebViewLongRunningScriptDetectedEventArgs args)
        {
            Debug.WriteLine(args.ExecutionTime);
        }

        private void _webView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            Debug.WriteLine("_webView_DOMContentLoaded");
        }

        private void _webView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            Debug.WriteLine("_webView_UnsafeContentWarningDisplaying");
        }

        private void _webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var content = e.Value;
            var form = new WwwFormUrlDecoder(content);
            var method = form.GetFirstValueByName("method");
            var param = form.GetFirstValueByName("param");
            var callback = form.GetFirstValueByName("callback");
            var task = Callback(method, param);
            if (null != task)
            {
                TaskCallback(task, callback);
            }
        }

        public async void LoadParesers()
        {
            var scriptsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("parsers", CreationCollisionOption.OpenIfExists);
            var scriptsFiles = await scriptsFolder.GetFilesAsync();
            var names = scriptsFiles.Select(f => "parsers/" + f.Name);
            var t= CallAsync("parserManager.load", string.Join(",", names));
        }

        [Init]
        public void Init()
        {
            _cache = new Dictionary<string, Func<string, Task<string>>>();
            AddCallback("Error", param =>
            {
                var t = new MessageDialog(param).ShowAsync();
                return null;
            });

            AddCallback("Log", param =>
            {
                Debug.WriteLine(param);
                return null;
            });

            AddCallback("HttpClientService.GetStringAsync", param =>
            {
                var task = _httpClientService.GetStringAsync(param);
                return task;
            });


            AddCallback("Play", param =>
            {
                _pluginParser.Callback(param);
                return null;
            });

            AddCallback("PlayAll", param =>
            {
                _pluginParser.CallbackJson(param);
                return null;
            });

            AddCallback("LoadParesers", param =>
            {
                LoadParesers();
                return null;
            });

            AddCallback("SecurityKit.ComputeMd5", param =>
            {
                var result = SecurityKit.ComputeMd5(param);
                return Task.FromResult(result);
            });

        }

        public void TaskCallback<T>(Task<T> task, string callback)
        {
            var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            task.ContinueWith(t => { var tmp = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { var x = CallAsync(callback, t.Result as string).ConfigureAwait(false); }); });
        }

        #region  IExternalInterface

        public void AddCallback(string javaScriptFunc, Func<string, Task<string>> func)
        {
            if (_cache.ContainsKey(javaScriptFunc))
            {
                Debug.WriteLine("replace javaScriptFunc:{0}", javaScriptFunc);
                _cache.Remove(javaScriptFunc);
            }
            _cache.Add(javaScriptFunc, func);
        }

        public Task<string> Callback(string functionName, string param)
        {
            Func<string, Task<string>> func;
            if (_cache.TryGetValue(functionName, out func))
            {
                var result = func.Invoke(param);
                return result;
            }

            throw new ServiceException(string.Format("no function:{0}", param));
        }


        public async Task<string> CallAsync(params string[] arguments)
        {
            var funcName = arguments.First();
            string result;

            if (funcName.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                result = await _webView.InvokeScriptAsync("execute", arguments);
            }
            else
            {
                var array = arguments.Skip(1);
                result = await _webView.InvokeScriptAsync(funcName, arguments.Skip(1));
            }
            return result;
        }

        #endregion
    }
}