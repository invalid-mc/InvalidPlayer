using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace InvalidPlayer.Parser.Iqiyi
{
    public sealed class DownloadService
    {
        private bool _hasSetHeader;
        private static DownloadService _instance;
        public static DownloadService Instance
        {
            get { return _instance ?? (_instance = new DownloadService()); }
        }

        private readonly HttpClient _httpClient;

        private DownloadService()
        {
            _httpClient = new HttpClient();
        }

        private async Task EnsureAddHeaders()
        {
            if (!_hasSetHeader)
            {
                await IqiyiHelper.AddSignHeaders(_httpClient.DefaultRequestHeaders);
                _hasSetHeader = true;
            }
        }

        public async Task<string> GetIface2JsonAsync(ApiParam param)
        {
            await EnsureAddHeaders();
            var uri = new Uri("http://iface2.iqiyi.com/php/xyz/entry/" + param.ApiName + ".php?" + param.QueryString);
            return await _httpClient.GetStringAsync(uri);
        } 
    }
}
