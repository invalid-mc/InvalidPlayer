using System.Collections.Generic;
using System.Threading.Tasks;
using InvalidPlayerCore.Model;

namespace InvalidPlayerCore.Parser
{
    /// <summary>
    ///     Video Parser
    /// </summary>
    public interface IVideoParser
    {
        /// <summary>
        ///     获取视频列表
        /// </summary>
        /// <param name="url">需要解析的URL</param>
        /// <returns>视频列表</returns>
        Task<List<VideoItem>> ParseAsync(string url);
    }
}