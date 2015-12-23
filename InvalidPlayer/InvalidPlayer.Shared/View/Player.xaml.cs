using System;
using System.IO;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using InvalidPlayer.Parser;
using InvalidPlayer.Parser.Youku;
using SYEngineCore;

namespace InvalidPlayer.View
{
    public sealed partial class Player : Page
    {
        private readonly IVideoUrlParser _youkuVideoUrlParser;

        public Player()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _youkuVideoUrlParser = new YoukuParser();
        }

        private void player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
        }

        private void player_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            BufferingProgressTextBlock.Text = $"Downloaded {MainPlayer.DownloadProgress*100}%";
        }

        private async void YoukuBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var youkuUrl = WebUrlTextBox.Text;
            if (!string.IsNullOrEmpty(youkuUrl))
            {
                if (MainPlayer.CurrentState == MediaElementState.Playing)
                {
                    MainPlayer.Stop();
                }
                try
                {
                    var videos = await _youkuVideoUrlParser.ParseAsync(youkuUrl);
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
                            plist.Append(video.Url, video.Size, video.Seconds);
                        }
                        var s = "plist://WinRT-TemporaryFolder_" +
                                Path.GetFileName(await plist.SaveAndGetFileUriAsync());
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
}