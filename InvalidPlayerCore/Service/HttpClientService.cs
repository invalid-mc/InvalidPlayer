using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using InvalidPlayerCore.Container;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvalidPlayerCore.Service
{
    [Singleton]
    public class HttpClientService
    {
        public HttpClientService()
        {
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient { get; private set; }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage message)
        {
            return await HttpClient.SendRequestAsync(message);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            return await HttpClient.SendRequestAsync(message).AsTask(cancellationToken);
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await HttpClient.GetStringAsync(new Uri(url));
        }

        public async Task<JToken> GetJsonAsync(string url)
        {
            return await GetJsonAsync(url, null, null);
        }

        public async Task<T> GetJsonDeserializeObjectAsync<T>(string url)
        {
            return await GetJsonDeserializeObjectAsync<T>(url, null, null);
        }

        public async Task<T> GetJsonDeserializeObjectAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> headers, string cookie = null)
        {
            var result = default(T);
            var content = await GetAsync(url, headers, cookie);
            if (content != null)
            {
                using (var s = (await content.ReadAsInputStreamAsync()).AsStreamForRead())
                using (var sr = new StreamReader(s))
                using (var reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    result = serializer.Deserialize<T>(reader);
                }
            }
            return result;
        }

        public async Task<JToken> GetJsonAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, string cookie = null)
        {
            JToken result = null;
            var content = await GetAsync(url, headers, cookie);
            if (content != null)
            {
                using (var s = (await content.ReadAsInputStreamAsync()).AsStreamForRead())
                using (var sr = new StreamReader(s))
                using (var reader = new JsonTextReader(sr))
                {
                    result = JToken.ReadFrom(reader);
                }
            }
            return result;
        }

        public async Task<string> GetStringAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, string cookie = null)
        {
            var content = await GetAsync(url, headers, cookie);
            if (null == content)
            {
                return null;
            }
            var result = await content.ReadAsStringAsync();
            return result;
        }


        public async Task<IInputStream> GetInputStreamAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, string cookie = null)
        {
            var content = await GetAsync(url, headers, cookie);
            if (null == content)
            {
                return null;
            }
            var result = await content.ReadAsInputStreamAsync();
            return result;
        }

        public async Task<IHttpContent> GetAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, string cookie = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Cookie.ParseAdd(cookie);
            }

            foreach (var valuePair in headers)
            {
                request.Headers.Add(valuePair);
            }

            var response = await HttpClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = response.Content;

            return result;
        }


        public async Task<string> GetStringAsync(string url, Encoding encoding)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            var response = await HttpClient.SendRequestAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            if (null == encoding)
            {
                encoding = Encoding.UTF8;
            }

            string result = null;

            using (var reader = new StreamReader((await response.Content.ReadAsInputStreamAsync()).AsStreamForRead(0), Encoding.UTF8))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }
    }
}