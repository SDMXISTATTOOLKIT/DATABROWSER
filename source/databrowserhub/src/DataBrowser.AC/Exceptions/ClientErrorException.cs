using System;
using DataBrowser.Interfaces.Exceptions;

namespace DataBrowser.AC.Exceptions
{
    public class ClientErrorException : Exception, IClientErrorException
    {
        public ClientErrorException(string errorCode, string message, bool showMessage = true) : base(
            $"[{errorCode}]:\t{message}")
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
            ShowMessage = showMessage;
        }

        public ClientErrorException(string errorCode, string message, Exception innerException, bool showMessage = true)
            : base($"[{errorCode}]:\t{message}", innerException)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
            ShowMessage = showMessage;
        }

        public ClientErrorException()
        {
        }

        public ClientErrorException(string message) : base(message)
        {
        }

        public ClientErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool ShowMessage { get; set; }
    }
}