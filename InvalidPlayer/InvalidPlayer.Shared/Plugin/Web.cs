using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web;

namespace YukiRemoteManger.View.Help
{
    public sealed class Web : IUriToStreamResolver
    {
        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            return AsyncInfo.Run<IInputStream>(async x =>
            {
                var url = uri.AbsolutePath;
                if (url.StartsWith("/parsers"))
                {
                    url = "ms-appdata:///local" + url;
                }
                else
                {
                    url = "ms-appx:///Plugin/JsCore" + url;
                }
                Debug.WriteLine(url);
                var path = new Uri(url, UriKind.Absolute);
                var fileRead = await StorageFile.GetFileFromApplicationUriAsync(path);
                return await fileRead.OpenAsync(FileAccessMode.Read);
            });
        }
    }
}