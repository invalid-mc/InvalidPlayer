using System.Threading.Tasks;
using InvalidPlayerCore.Model;

namespace InvalidPlayerCore.Parser
{
    public interface ISubtitlesSupport
    {
        Task<SubtitlesInfo> QuerySubtitlesAsync(string webUrl, string name);
    }
}