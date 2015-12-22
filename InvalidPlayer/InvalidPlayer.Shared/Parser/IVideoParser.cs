using System.Collections.Generic;
using System.Threading.Tasks;
using InvalidPlayer.Model;

namespace InvalidPlayer.Parser
{
    public interface IVideoParser
    {
        Task<List<VideoItem>> ParseAsync(string url);
    }
}