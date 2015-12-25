﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

            this.Loaded += delegate
            {
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            };
            this.Unloaded += delegate
            {
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;
            };
            this.SizeChanged += delegate
            {
                UpdateInfo();
            };
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            Debug.WriteLine("CoreW: " + MainPlayer.IsFullWindow + "\t" + e.VirtualKey);
            var elem = FocusManager.GetFocusedElement();
            Debug.WriteLine(elem);
            if (elem == WebUrlTextBox || elem == SearchBtn) return;

            if (e.VirtualKey == VirtualKey.F1)
            {
                SetInfoVisible(true);
            }
            else if (e.VirtualKey == VirtualKey.Enter)
            {
                if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down)
                    MainPlayer.IsFullWindow = true;
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            var elem = FocusManager.GetFocusedElement();
            if (elem == WebUrlTextBox || elem == SearchBtn) return;

            if (e.VirtualKey == VirtualKey.F1)
            {
                SetInfoVisible(false);
            }
            else if (e.VirtualKey == VirtualKey.Escape)
            {
                MainPlayer.IsFullWindow = false;
            }
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SetInfoVisible(false);
        }

        private void player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }

        private void player_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }

        private void player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            InputGrid.Visibility = MainPlayer.CurrentState == MediaElementState.Playing
                ? Visibility.Collapsed
                : Visibility.Visible;
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

        private void SetInfoVisible(bool visible)
        {
            if (visible && MainPlayer.Source != null)
            {
                InfoPanel.Visibility = Visibility.Visible;
                UpdateInfo();
            }
            else
            {
                InfoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateInfo(bool force = false)
        {
            if(!force && InfoPanel.Visibility== Visibility.Collapsed) return;
            
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
                    cfgs.UniqueId = DateTime.UtcNow.ToString();
                    cfgs.DownloadRetryOnFail = true;
                    cfgs.DetectDurationForParts = false;
                    cfgs.HttpUserAgent = string.Empty;
                    cfgs.HttpReferer = string.Empty;
                    cfgs.HttpCookie = string.Empty;
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
                new MessageDialog(exception.Message).ShowAsync();
            }
        }
    }
}