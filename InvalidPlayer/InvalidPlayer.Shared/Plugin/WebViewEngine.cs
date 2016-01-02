using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using InvalidPlayer.Plugin;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Service;

#pragma warning disable 649

namespace InvalidPlayerCore.Plugin.JavascriptEngine
{
    [Singleton]
    public class WebViewEngine
    {
        private readonly WebView _webView;

        [Inject]
        private HttpClientService _httpClientService;

        [Inject]
        private PluginParser _pluginParser;


        public WebViewEngine()
        {
            _webView = new WebView();
            _webView.ScriptNotify += _webView_ScriptNotify;
            var url = "ms-appx-web:///Plugin/JsCore/Main.html";
            _webView.Source = new Uri(url);
        }

        private void _webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var content = e.Value;
            var form = new WwwFormUrlDecoder(content);
            var method = form.GetFirstValueByName("method");
            var param = form.GetFirstValueByName("param");
            var callback = form.GetFirstValueByName("callback");
            //TODO test only
            switch (method)
            {
                case "HttpClientService.GetStringAsync":
                {
                    var task = _httpClientService.GetStringAsync(param);
                    if (!string.IsNullOrEmpty(callback))
                    {
                        TaskCallback(task, callback);
                    }
                    break;
                }

                case "Play":
                {
                    //TODO
                    _pluginParser.Callback(param);
                    break;
                }
                case "Error":
                {
                    var t = new MessageDialog(param).ShowAsync();
                        break;
                    }
                case "Log":
                    {
                        Debug.WriteLine(param);
                        break;
                    }
                    break;
            }
        }


        public void TaskCallback<T>(Task<T> task, string callback)
        {
            task.ContinueWith(async t => { await ExecuteMethodAsync(callback, t.Result as string); });
        }

        public async Task<string> ExecuteMethodAsync(string funcName, params string[] args)
        {
            if (funcName.IndexOf(".") != -1)
            {
                var wf = funcName.Replace(".", "");
                var s = wf + "=" + funcName;
              //  await _webView.InvokeScriptAsync("eval", new [] {s});
              //  funcName = wf;
            }
            return await _webView.InvokeScriptAsync(funcName, args);
        }
    }
}