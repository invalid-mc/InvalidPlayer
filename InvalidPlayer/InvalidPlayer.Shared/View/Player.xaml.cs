using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Service;
using SYEngine;
using Windows.ApplicationModel;

namespace InvalidPlayer.View
{

    public sealed partial class Player : Page

    {
        [Inject("SimpleVideoPlayerFactory")]
        private IVideoPlayerFactory _playerFactory;

        private IVideoPlayer _videoPlayer;

        public Player()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            SYEngine.Core.PlaylistSegmentDetailUpdateDelegate += Core_PlaylistSegmentDetailUpdateEvent;
            Unloaded += Player_Unloaded;

            InitInfo();
            SetLayout();

            StaticContainer.AutoInject(this);
        }

        private void SetLayout()
        {

            Grid.SetRow(this.BtnPanel, 0);
            Grid.SetColumn(this.BtnPanel, 1);
        }

        private void Player_Unloaded(object sender, RoutedEventArgs e)
        {
            Core.PlaylistSegmentDetailUpdateDelegate -= Core_PlaylistSegmentDetailUpdateEvent;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var uri = e.Parameter as Uri;
            if (uri != null)
            {
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
            try
            {
                if (_videoPlayer.TryRefreshSegment(curIndex, totalCount, out var detail))
                {
                    var header = detail.Header;
                    foreach (var pair in header)
                    {
                        info.SetRequestHeader(pair.Key, pair.Value);
                    }
                    Debug.WriteLine("update url success \n form {0} \n to {1}", info.Url, detail.Url);
                    info.Url = detail.Url;
                    return true;
                }
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e.Message);
            }

            return false;
        }

        private async void OpenBtn_OnClick(object sender, RoutedEventArgs e)
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
                _videoPlayer = _playerFactory.GetPlayer(url);
                await _videoPlayer.StartPlayVideo(MainPlayer);
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception.Message);
            }
        }

        private async void ShowExceptionMessage(string message)
        {
            await new MessageDialog(message).ShowAsync();
        }

        private async void AboutBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var version = Package.Current.Id.Version;
            var contentDialog = new ContentDialog();
            contentDialog.Title = "About";
            contentDialog.CloseButtonText = "Close";
            contentDialog.Content = $"{Package.Current.DisplayName}\nVersion: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            _ = contentDialog.ShowAsync();
        }

        private async void LocalBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker { CommitButtonText = "播放", SuggestedStartLocation = PickerLocationId.VideosLibrary };
            filePicker.FileTypeFilter.Add(".mp4");
            filePicker.FileTypeFilter.Add(".mkv");
            filePicker.FileTypeFilter.Add(".flv");
            var file = await filePicker.PickSingleFileAsync();
            await PlayLocalFile(file);
        }


        private async Task PlayLocalFile(StorageFile file)
        {
            if (file != null)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);
                await Play(file.Path);
            }
        }

    }
}