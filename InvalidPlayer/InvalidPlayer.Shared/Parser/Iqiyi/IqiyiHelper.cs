using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using InvalidPlayer.Service;

namespace InvalidPlayer.Parser.Iqiyi
{
    public static class IqiyiHelper
    {
        private const int TimestampKey = 1771171717;
        private const string SignKey = "aXFpWUkzOV9fIzEwMDg2MTA5MTQ4MTYyMThhMjE2Nzg0YzQzYTA4YWFh";

        private static int CurrentUnixTimeMillis()
        {
            //return (DateTime.UtcNow.Ticks - 621355968000000000)/10000;
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static async Task<Tuple<string, string>> GetSignHeader()
        {
            var num = CurrentUnixTimeMillis();
            var t = (num ^ TimestampKey).ToString();
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(num);
            var keyBytes =Convert.FromBase64String(SignKey);
            stringBuilder.Append(Encoding.UTF8.GetString(keyBytes,0, keyBytes.Length));
            stringBuilder.Append(await GetAppVersion());
            var token = stringBuilder.ToString();
            var sign = SecurityKit.ComputeMD5(token);
            return new Tuple<string, string>(t, sign);
        }

        public static async Task AddSignHeaders(HttpRequestHeaderCollection requestHeader)
        {
            var header = await GetSignHeader();
            requestHeader.Add("t", header.Item1);
            requestHeader.Add("sign", header.Item2);
        }

        private static Task<string> GetAppVersion()
        {
            //https://www.microsoft.com/store/apps/9wzdncrfj15n
            var version = "3.2.2.2";
            var verArray = version.Split('.');
            // 包的主版本号。
            //public ushort Major;
            // 包的主版本号。
            // public ushort Minor;
            // 包的内部版本号。
            // public ushort Build;
            // 包的修订版本号。
            // public ushort Revision;
            var stringBuilder = new StringBuilder().Append(verArray[0]).Append('.').Append(verArray[1]);
            if (int.Parse(verArray[2]) > 0)
            {
                stringBuilder.Append('.').Append(verArray[2]);
            }
            return Task.FromResult(stringBuilder.ToString());
        }
    }
}