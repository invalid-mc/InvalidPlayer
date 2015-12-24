using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using InvalidPlayer.Parser;
using SYEngineCore;

namespace InvalidPlayer.View
{
    public sealed partial class Player : Page
    {
        private readonly IVideoParser _videoParser;

        public Player()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _videoParser = new VideoParser();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //weburl://?url=http://v.youku.com/v_show/id_XMTM1MTUzOTY1Ng==.html
            if (e.Parameter is Uri)
            {
                var uri = (Uri)e.Parameter;
                var query = uri.Query;
                if (!string.IsNullOrEmpty(query))
                {
                    var form = new WwwFormUrlDecoder(query);
                    var url = form.GetFirstValueByName("url");
                    if (!string.IsNullOrEmpty(url))
                    {
                        WebUrlTextBox.Text = url;
                        await Play(url);
                    }
                }
            }
        }

        private void player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            //BufferingProgressTextBlock.Text = $"{MainPlayer.BufferingProgress*100}%";
            PlayInfo.Text = string.Format(PlayInfoStr,
                (int)MainPlayer.ActualWidth, (int)MainPlayer.ActualHeight,
                MainPlayer.BufferingProgress * 100, (MainPlayer.DownloadProgress * 100).ToString("F"),
                MainPlayer.DownloadProgressOffset, MainPlayer.PlaybackRate);
        }

        private void player_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
        }

        private void player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            InputGrid.Visibility = MainPlayer.CurrentState == MediaElementState.Playing
                ? Visibility.Collapsed
                : Visibility.Visible;

            if (MainPlayer.CurrentState != MediaElementState.Playing &&
                MainPlayer.CurrentState != MediaElementState.Buffering)
            {
                InfoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void player_OnKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                MainPlayer.IsFullWindow = false;
            }
            else if (e.Key == VirtualKey.Tab)
            {
                e.Handled = true;
                OnTabPress(true);
            }
        }

        private void player_OnKeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                e.Handled = true;
                OnTabPress(false);
            }
            else if (e.Key == VirtualKey.Enter && e.KeyStatus.IsMenuKeyDown)
            {
                MainPlayer.IsFullWindow = true;
            }
        }

        private const string VideoInfoStr = "原始分辨率：{0} * {1}\n" +
                                            "视频纵横比：{2} * {3}\n" +
                                            "视频长度：{4}\n" +
                                            "是否 3D：{5}\n" +
                                            "音频流数目：{6}\n" +
                                            "当前使用音频流索引：{7}\n" +
                                            "音频流名称：{8}\n" +
                                            "视频源：{9}\n";

        private const string PlayInfoStr = "渲染分辨率：{0} * {1}\n" +
                                           "当前缓冲进度：{2}%\n" +
                                           "已下载：{3}%\n" +
                                           "下载进度偏移：{4}\n" +
                                           "播放速率：{5}\n";

        private const string ElemInfoStr = "自动播放：{0}\n" +
                                           "重复播放：{1}\n" +
                                           "默认播放速率：{2}\n" +
                                           "视频拉伸方式：{3}\n";

        private void OnTabPress(bool isPressed)
        {
            if (MainPlayer.CurrentState == MediaElementState.Playing ||
                MainPlayer.CurrentState == MediaElementState.Buffering)
            {
                InfoPanel.Visibility = isPressed ? Visibility.Visible : Visibility.Collapsed;
                VideoInfo.Text = string.Format(VideoInfoStr,
                    MainPlayer.NaturalVideoWidth, MainPlayer.NaturalVideoHeight,
                    MainPlayer.AspectRatioWidth, MainPlayer.AspectRatioHeight,
                    MainPlayer.NaturalDuration, MainPlayer.IsStereo3DVideo,
                    MainPlayer.AudioStreamCount, MainPlayer.AudioStreamIndex,
                    MainPlayer.GetAudioStreamLanguage(MainPlayer.AudioStreamIndex), MainPlayer.Source);
                PlayInfo.Text = string.Format(PlayInfoStr,
                    (int)MainPlayer.ActualWidth, (int)MainPlayer.ActualHeight,
                    MainPlayer.BufferingProgress * 100, (MainPlayer.DownloadProgress * 100).ToString("F"),
                    MainPlayer.DownloadProgressOffset, MainPlayer.PlaybackRate);
                MediaElemInfo.Text = string.Format(ElemInfoStr,
                    MainPlayer.AutoPlay, MainPlayer.IsLooping,
                    MainPlayer.DefaultPlaybackRate, MainPlayer.Stretch);
            }
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
            if (MainPlayer.CurrentState == MediaElementState.Playing)
            {
                MainPlayer.Stop();
            }

            try
            {
                var videos = await _videoParser.ParseAsync(url);
                if (videos.Count > 1)
                {
                    var plist = new Playlist(PlaylistTypes.NetworkHttp);
                    var cfgs = default(PlaylistNetworkConfigs);
                    cfgs.DownloadRetryOnFail = true;
                    cfgs.DetectDurationForParts = false;
                    cfgs.HttpUserAgent = string.Empty;
                    cfgs.HttpReferer = string.Empty;
                    cfgs.HttpCookie = string.Empty;
                    cfgs.TempFilePath = string.Empty;
                    plist.NetworkConfigs = cfgs;
                    foreach (var video in videos)
                    {
                        plist.Append(video.Url, video.Size, (float)video.Seconds);
                    }
                    var s = "plist://WinRT-TemporaryFolder_" + Path.GetFileName(await plist.SaveAndGetFileUriAsync());
                    MainPlayer.Source = new Uri(s);
                }
                else if (videos.Count == 1)
                {
                    MainPlayer.Source = new Uri(videos[0].Url);
                }
            }
            catch (Exception exception)
            {
                var t = new MessageDialog(exception.Message).ShowAsync();
            }
        }
    }
}