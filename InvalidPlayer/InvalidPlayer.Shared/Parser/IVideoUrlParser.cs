using System.Collections.Generic;
using System.Threading.Tasks;
using InvalidPlayer.Model;

namespace InvalidPlayer.Parser
{
    public interface IVideoUrlParser
    {
        Task<List<VideoItem>> ParseAsync(string url);
    }
}