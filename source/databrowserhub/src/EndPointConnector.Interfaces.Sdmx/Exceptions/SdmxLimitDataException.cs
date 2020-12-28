using System;
using EndPointConnector.Interfaces.Excepetions;

namespace EndPointConnector.Interfaces.Sdmx.Exceptions
{
    public class SdmxLimitDataException : Exception, ILimitDataException
    {
        public SdmxLimitDataException(string message)
            : base(message)
        {
        }
    }
}