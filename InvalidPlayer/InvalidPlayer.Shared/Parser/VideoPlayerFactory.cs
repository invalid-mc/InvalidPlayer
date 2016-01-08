using System;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Parser;

#pragma warning disable 649

namespace InvalidPlayer.Parser
{
    [Singleton("SimpleVideoPlayerFactory")]
    public class VideoPlayerFactory : IVideoPlayerFactory
    {
        [Inject("RegexVideoPlayer")]
        private IVideoPlayer _regexVideoPlayer;
        [Inject("LocalVideoPlayer")]
        private IVideoPlayer _localVideoPlayer;

        public IVideoPlayer GetPlayer(string url)
        {
            var uri = new Uri(url);
            if (uri.IsFile)
            {
                _localVideoPlayer.Url = url;
                return _localVideoPlayer;
            }
            else
            {
                _regexVideoPlayer.Url = url;
                return _regexVideoPlayer;
            }
        }
    }
}
