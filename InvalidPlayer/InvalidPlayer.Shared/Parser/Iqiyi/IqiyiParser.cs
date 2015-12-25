using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InvalidPlayer.Model;

namespace InvalidPlayer.Parser.Iqiyi
{
    public class IqiyiParser : IVideoParser
    {
        public string Name
        {
            get { return "iqiyi"; }
        }

        public async Task<List<VideoItem>> ParseAsync(string url)
        {

            var faceJson = await DownloadService.Instance.GetIface2JsonAsync(new ApiParam());
            return new List<VideoItem>();
        }
    }
}
