using System;

namespace InvalidPlayer.Service
{
    public class AssertUtil
    {
        public static void IsTrue(bool expression, string message)
        {
            if (!expression)
            {
                throw new Exception(message);
            }
        }

        public static void IsNull(object param, string message)
        {
            if (param != null)
            {
                throw new Exception(message);
            }
        }

        public static void NotNull(object param, string message)
        {
            if (param == null)
            {
                throw new Exception(message);
            }
        }

        public static void HasText(string text, string message)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception(message);
            }
        }

        public static void NotEmpty(object[] array, string message)
        {
            if (null == array || array.Length < 1)
            {
                throw new Exception(message);
            }
        }
    }
}