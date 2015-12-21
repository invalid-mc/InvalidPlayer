using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace InvalidPlayer.Parser.Youku
{
    public static class YoukuAes
    {
        public static string Decrypt(string data)
        {
            var symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var key = CryptographicBuffer.DecodeFromHexString("71776572336173326a696e3466647361");
            var cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key);
            var encoding = BinaryStringEncoding.Utf8;
            var buffMsg = CryptographicBuffer.DecodeFromBase64String(data);
            var result = CryptographicEngine.Decrypt(cryptoKey, buffMsg, null);
            return CryptographicBuffer.ConvertBinaryToString(encoding, result);
        }

        public static string Encrypt(string data)
        {
            var symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var key = CryptographicBuffer.DecodeFromHexString("36623064666262303762313961653363");
            var cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key);
            var encoding = BinaryStringEncoding.Utf8;
            var buffMsg = CryptographicBuffer.ConvertStringToBinary(Fill(data), encoding);
            var result = CryptographicEngine.Encrypt(cryptoKey, buffMsg, null);
            return CryptographicBuffer.EncodeToBase64String(result);
        }

        private static string Fill(string data)
        {
            var sb = new StringBuilder(data);
            var len = GetToFillCount(data.Length);
            sb.Append(' ', len);
            return sb.ToString();
        }

        private static int GetToFillCount(int length)
        {
            var num = length/32;
            return num == 0 ? 32 - length : 32*(num + 1) - length;
        }
    }
}