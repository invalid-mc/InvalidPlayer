using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using InvalidPlayerCore.Model;

namespace InvalidPlayerCore.Parser
{
    /// <summary>
    ///     提供IVideoParser调度
    /// </summary>
    public interface IVideoPlayer
    {
        /// <summary>
        ///     播放URL
        /// </summary>
        string Url { get; set; }
        
        /// <summary>
        ///     开始播放视频
        /// </summary>
        /// <param name="element">视频播放控件</param>
        Task StartPlayVideo(MediaElement element);

        /// <summary>
        ///     尝试刷新切片信息
        /// </summary>
        /// <param name="curIndex">需要刷新的切片索引</param>
        /// <param name="totalCount">切片总数</param>
        /// <param name="item">成功则返回视频信息，否则返回null</param>
        bool TryRefreshSegment(int curIndex, int totalCount, out DetailVideoItem item);
    }
}