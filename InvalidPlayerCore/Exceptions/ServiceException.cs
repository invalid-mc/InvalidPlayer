using System;

namespace InvalidPlayerCore.Exceptions
{
    public class ServiceException : Exception
    {
        public int ExceptionType { get; set; }=-1;

        public ServiceException(int type) 
        {
            ExceptionType = type;
        }


        public ServiceException(int type,string message, Exception exception) : base(message, exception)
        {
            ExceptionType = type;
        }

        public ServiceException(int type, string message) : base(message)
        {
            ExceptionType = type;
        }

        public ServiceException(string message, Exception exception) : base(message, exception)
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

       
    }
}