using System;

namespace DataBrowser.AC.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}