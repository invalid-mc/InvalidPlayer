using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvalidPlayerCore.Model;

namespace InvalidPlayerCore.Parser
{
     public  interface IVideoRefreshSupport
    {
        Task<DetailVideoItem> RefreshAsync(string url);
    }
}
