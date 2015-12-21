using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using InvalidPlayer.Parser;
using InvalidPlayer.Parser.Youku;

namespace InvalidPlayer.View
{
    public sealed partial class Player : Page
    {
        public Player()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _youkuVideoUrlParser = new YoukuParser();
        }
        private IVideoUrlParser _youkuVideoUrlParser;

        private void player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
       
        }

        private void player_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            BufferingProgressTextBlock.Text = $"Downloaded {MainPlayer.DownloadProgress * 100}%";
        }

        private async void YoukuBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var youkuUrl = WebUrlTextBox.Text;
            if (!string.IsNullOrEmpty(youkuUrl))
            {
                try
                {
                    var urls = await _youkuVideoUrlParser.ParseAsync(youkuUrl);
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
                    var s = "plist://WinRT-TemporaryFolder_" + System.IO.Path.GetFileName(await plist.SaveAndGetFileUriAsync());
                    MainPlayer.Source = new Uri(s);
                  
                }
                catch (Exception exception)
                {
                   var t= new MessageDialog(exception.Message).ShowAsync();
                }
            }
        }

    
    }
}
