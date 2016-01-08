using System.Text.RegularExpressions;

namespace InvalidPlayerCore.Service
{
    public static class RegexHelper
    {
        public static string GetMatchString(string source, Regex regex, int index)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }
            var match = regex.Match(source);
            if (match.Success)
            {
                return match.Groups[index].ToString();
            }
            return null;
        }

        public static string GetMatchString(string source, string pattern, int index)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            var match = Regex.Match(source, pattern);
            return match.Success ? match.Groups[index].ToString() : null;
        }
    }
}