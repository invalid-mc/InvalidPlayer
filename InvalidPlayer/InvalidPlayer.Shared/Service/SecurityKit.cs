using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Yuki.Common.Util
{
    public class SecurityKit
    {
        public static string ComputeMd5(string str, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            return ComputeHash(str, HashAlgorithmNames.Md5, encoding);
        }


        public static string ComputeMd5(IBuffer buffer)
        {
            var hashed = ComputeHashWithBuffer(buffer, HashAlgorithmNames.Md5);
            return CryptographicBuffer.EncodeToHexString(hashed);
        }

        private static string ComputeHash(string str, string hashAlgorithmName, BinaryStringEncoding encoding)
        {
            var buff = CryptographicBuffer.ConvertStringToBinary(str, encoding);
            var hashed = ComputeHashWithBuffer(buff, hashAlgorithmName);
            return CryptographicBuffer.EncodeToHexString(hashed);
        }

        private static IBuffer ComputeHashWithBuffer(IBuffer buffer, string hashAlgorithmName)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(hashAlgorithmName);
            var hashed = alg.HashData(buffer);
            return hashed;
        }

        public static string ComputeSha1(string str, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            return ComputeHash(str, HashAlgorithmNames.Sha1, encoding);
        }

        public static string ComputeSha256(string str, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
        {
            return ComputeHash(str, HashAlgorithmNames.Sha256, encoding);
        }

        public static async Task<string> ComputeMd5(IInputStream stream, BinaryStringEncoding encoding = BinaryStringEncoding.Utf8)
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