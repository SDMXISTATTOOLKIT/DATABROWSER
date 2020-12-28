using System;

namespace DataBrowser.AC.Exceptions
{
    public class InsufficentPermissionException : Exception
    {
        public InsufficentPermissionException()
        {
        }

        public InsufficentPermissionException(string message) : base(message)
        {
        }

        public InsufficentPermissionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}