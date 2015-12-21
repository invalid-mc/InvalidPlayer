using System;
using System.IO;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InvalidPlayer.Parser;
using InvalidPlayer.Parser.Youku;

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
                try
                {
                    var urls = await _youkuVideoUrlParser.ParseAsync(youkuUrl);
                    if (urls.Count > 1)
                    {
                        SYEngineCore.Playlist plist = new SYEngineCore.Playlist(SYEngineCore.PlaylistTypes.NetworkHttp);
                        SYEngineCore.PlaylistNetworkConfigs cfgs = default(SYEngineCore.PlaylistNetworkConfigs);
                        cfgs.DownloadRetryOnFail = true;
                        cfgs.DetectDurationForParts = true;
                        cfgs.HttpUserAgent = string.Empty;
                        cfgs.HttpReferer = string.Empty;
                        cfgs.HttpCookie = string.Empty;
                        cfgs.TempFilePath = string.Empty;
                        plist.NetworkConfigs = cfgs;
                        foreach (var url in urls)
                        {
                            plist.Append(url, 0, 0);
                        }
                        var s = "plist://WinRT-TemporaryFolder_" +
                                Path.GetFileName(await plist.SaveAndGetFileUriAsync());
                        MainPlayer.Source = new Uri(s);
                    }
                    else if (urls.Count == 1)
                    {
                        MainPlayer.Source = new Uri(urls[0]);
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