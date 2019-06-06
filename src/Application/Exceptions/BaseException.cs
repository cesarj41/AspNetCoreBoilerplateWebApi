using System;

namespace Application.Exceptions
{
    public class BaseException : Exception
    {
        public int StatusCode { get; private set; }
        public string[] Errors { get; private set; }

        public BaseException(string message, int statusCode) : base(message)
        {
            this.Errors = new string[] { message };
            this.StatusCode = statusCode;
        }

    }
}