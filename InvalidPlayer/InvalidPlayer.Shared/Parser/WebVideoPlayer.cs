using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using InvalidPlayerCore.Model;
using InvalidPlayerCore.Parser;
using Windows.UI.Xaml.Controls;
using InvalidPlayerCore.Service;
using System.IO;
using SYEngine;

namespace InvalidPlayer.Parser
{
    public abstract class WebVideoPlayer : IVideoPlayer
    {
        protected IVideoParser Parser;
        protected List<VideoItem> Videos;

        public abstract string Url { get; set; }

        public virtual async Task StartPlayVideo(MediaElement element)
        {
            AssertUtil.NotNull(Parser, "unsupport url");

            Videos = await Parser.ParseAsync(Url);
            AssertUtil.NotNull(Videos, "no videos");
#if DEBUG
            foreach (var videoItem in Videos)
            {
                Debug.WriteLine(videoItem);
            }
#endif
            if (Videos.Count > 1)
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
                foreach (var video in Videos)
                {
                    plist.Append(video.Url, video.Size, (float)video.Seconds);
                }
#if DEBUG
                var debugFile = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "DebugFile.mkv");
                Debug.WriteLine(string.Format("DebugFile File:{0}", debugFile));
                plist.SetDebugFile(debugFile);
#endif
                var uri = await plist.SaveAndGetFileUriAsync();
                element.Source = uri;
            }
            else if (Videos.Count == 1)
            {
                element.Source = new Uri(Videos[0].Url);
            }
        }

        public virtual bool TryRefreshSegment(int curIndex, int totalCount, out DetailVideoItem item)
        {
            item = null;
            var refreshParser = Parser as IVideoRefreshSupport;
            if (null == refreshParser) return false;

            if (null == Videos || Videos.Count - 1 > totalCount || Videos.Count <= curIndex)
            {
                return false;
            }
            Debug.WriteLine("update url for index {0}", curIndex);

            var url = Videos[curIndex].Url;
            item = refreshParser.RefreshAsync(url).Result;
            return item != null;
        }
    }
}
