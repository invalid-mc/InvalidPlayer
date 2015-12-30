using System;

namespace InvalidPlayerCore.Exceptions
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message)
        {
        }

        public AppException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}