using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvalidPlayer.Parser
{
    public interface IVideoUrlParser
    {
        Task<List<string>> ParseAsync(string url);
    }
}