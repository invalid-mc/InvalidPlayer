using System;
using System.Threading.Tasks;
using Windows.Storage;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using Windows.UI.Xaml.Controls;

namespace InvalidPlayer.Parser
{
    [Singleton("LocalVideoPlayer")]
    public class LocalVideoPlayer : IVideoPlayer
    {
        public string Url { get; set; }

        public async Task StartPlayVideo(MediaElement element)
        {
            var file = await StorageFile.GetFileFromPathAsync(Url);
            element.SetSource(await file.OpenAsync(FileAccessMode.Read), "");
        }

        public bool TryRefreshSegment(int curIndex, int totalCount, out DetailVideoItem item)
        {
            item = null;
            return false;
        }
    }
}
