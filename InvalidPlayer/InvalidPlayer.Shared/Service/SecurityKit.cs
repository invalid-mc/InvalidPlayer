using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace InvalidPlayer.Service
{
    public class SecurityKit
    {
        public static string ComputeMD5(string str, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            return ComputeHash(str, alg, encoding);
        }

        private static string ComputeHash(string str, HashAlgorithmProvider alg, BinaryStringEncoding encoding)
        {
            var buff = CryptographicBuffer.ConvertStringToBinary(str, encoding);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        public static string ComputeSha1(string str, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            return ComputeHash(str, alg, encoding);
            ;
        }

        public static string ComputeSha256(string str, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            return ComputeHash(str, alg, encoding);
            ;
        }

        public static async Task<string> ComputeMD5(IInputStream stream,BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            var hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var hash = hashAlgorithmProvider.CreateHash();
            uint size = 1024*64;
            var buffer = new Buffer(size);
            while (true)
            {
                var x = await stream.ReadAsync(buffer, size, InputStreamOptions.Partial);
                if (x.Length < 1)
                {
                    break;
                }
                hash.Append(x);
            }

            var result = hash.GetValueAndReset();
            var hex = CryptographicBuffer.EncodeToHexString(result);
            return hex;
        }
    }
}