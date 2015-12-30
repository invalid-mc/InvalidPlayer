using System.Threading.Tasks;
using InvalidPlayerCore.Model;

namespace InvalidPlayerCore.Parser
{
    public interface IVideoRefreshSupport
    {
        Task<DetailVideoItem> RefreshAsync(string url);
    }
}