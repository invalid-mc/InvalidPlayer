using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using InvalidPlayerCore.Container;

#pragma warning disable 649

namespace InvalidPlayerCore.Service
{
    [Singleton]
    public class AppService
    {
        private const string RamdomWebUrlApi = "";

        [Inject] private HttpClientService _httpClientService;

        public async Task<string> GetRamdomWebUrl()
        {
            var result = await _httpClientService.GetJsonAsync(RamdomWebUrlApi);
            if (null == result)
            {
                return null;
            }
            return null;
        }

        public List<bool> CheckVerison()
        {
            throw new NotImplementedException();
        }

        public List<string> GetServerVersion()
        {
            throw new NotImplementedException();
        }

        public int GetLocalVersion()
        {
            var verson = Package.Current.Id.Version;
            var num = verson.Build*1000 + verson.Major*100 + verson.Minor*10 + verson.Revision;
            return num;
        }
    }
}