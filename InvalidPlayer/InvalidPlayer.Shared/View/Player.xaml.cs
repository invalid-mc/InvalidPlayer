using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Service;
using SYEngineCore;

namespace InvalidPlayer.View
{
    public sealed partial class Player : Page
    {
        private IVideoParser _parser;

        [Inject] private IVideoParserDispatcher _videoParser;

        private List<VideoItem> _videos;

        public Player()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            InitInfo();
            Core.PlaylistSegmentDetailUpdateEvent += Core_PlaylistSegmentDetailUpdateEvent;
            StaticContainer.AutoInject(this);
            Unloaded += Player_Unloaded;
        }

        private void Player_Unloaded(object sender, RoutedEventArgs e)
        {
            Core.PlaylistSegmentDetailUpdateEvent -= Core_PlaylistSegmentDetailUpdateEvent;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Uri)
            {
                var uri = (Uri) e.Parameter;
                await OpenFromUri(uri);
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SetInfoVisible(false);
            DisplayRequestUtil.RequestRelease();
        }

        private async Task OpenFromUri(Uri uri)
        {
            var query = uri.Query;
            if (string.IsNullOrEmpty(query))
            {
                return;
            }
            var form = new WwwFormUrlDecoder(query);
            var url = "";
            var cookie = "";
            foreach (var item in form)
            {
                if (item.Name == "url")
                {
                    url = item.Value;
                }
                else if (item.Name == "cookie")
                {
                    cookie = item.Value;
                }
            }
            if (!string.IsNullOrEmpty(url))
            {
                if (!string.IsNullOrEmpty(cookie))
                {
                    url += "#cookie=" + cookie;
                }
                WebUrlTextBox.Text = url;
                await Play(url);
            }
        }

        private bool Core_PlaylistSegmentDetailUpdateEvent(string uniqueId, string opType, int curIndex, int totalCount, IPlaylistNetworkUpdateInfo info)
        {
            var refreshParser = _parser as IVideoRefreshSupport;
            if (null == refreshParser)
            {
                return false;
            }

            Debug.WriteLine("update url for index {0}", curIndex);

            if (null == _videos || _videos.Count - 1 > totalCount || _videos.Count <= curIndex)
            {
                return false;
            }

            try
            {
                var url = _videos[curIndex].Url;
                var detail = refreshParser.RefreshAsync(url).Result;
                var header = detail.Header;
                foreach (var pair in header)
                {
                    info.SetRequestHeader(pair.Key, pair.Value);
                }
                Debug.WriteLine("update url success \n form {0} \n to {1}", url, detail.Url);
                info.Url = detail.Url;
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e.Message);
                return false;
            }

            return true;
        }


        private async void YoukuBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var url = WebUrlTextBox.Text;
            if (!string.IsNullOrEmpty(url))
            {
                await Play(url);
            }
        }

        private async Task Play(string url)
        {
            DisplayRequestUtil.RequestActive();

            try
            {
                await GetVideo(url);
                if (_videos.Count > 1)
                {
                    var plist = new Playlist(PlaylistTypes.NetworkHttp);
                    var cfgs = default(PlaylistNetworkConfigs);
                    cfgs.UniqueId = DateTime.UtcNow.ToString();
                    cfgs.DownloadRetryOnFail = true;
                    cfgs.DetectDurationForParts = false;
                    cfgs.HttpUserAgent = string.Empty;
                    cfgs.HttpReferer = string.Empty;
                    cfgs.HttpCookie = string.Empty;
                    plist.NetworkConfigs = cfgs;
                    foreach (var video in _videos)
                    {
                        plist.Append(video.Url, video.Size, (float) video.Seconds);
                    }

                    var s = "plist://WinRT-TemporaryFolder_" + Path.GetFileName(await plist.SaveAndGetFileUriAsync());
                    MainPlayer.Source = new Uri(s);
                }
                else if (_videos.Count == 1)
                {
                    MainPlayer.Source = new Uri(_videos[0].Url);
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        private async Task GetVideo(string url)
        {
            _parser = _videoParser.GetParser(url);
            AssertUtil.NotNull(_parser, "unsupport url");
            _videos = await _parser.ParseAsync(url);
            AssertUtil.NotNull(_videos, "no videos");
#if DEBUG
            foreach (var videoItem in _videos)
            {
                Debug.WriteLine(videoItem);
            }
#endif
        }

        private void ShowExceptionMessage(string message)
        {
            var t = new MessageDialog(message).ShowAsync();
        }

        private void AboutBtn_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}