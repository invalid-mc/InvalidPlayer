using System.Collections.Generic;
using System.Threading.Tasks;
using InvalidPlayer.Model;

namespace InvalidPlayer.Parser
{
    /// <summary>
    ///     Video Parser
    /// </summary>
    public interface IVideoParser
    {
        /// <summary>
        ///     Name标志
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     获取视频列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<List<VideoItem>> ParseAsync(string url);
    }
}