namespace Sister.EndPointConnector.Sdmx.Constants
{
    public class SdmxSoapConstants
    {
        /// <summary>
        ///     SOAP 1.1 namespace
        /// </summary>
        public const string Soap11Ns = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        ///     This field holds a template for soap 1.1 request envelope
        /// </summary>
        public const string SoapRequest =
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:{0}=\"{1}\"><soap:Header/><soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        ///     SOAP Body tag
        /// </summary>
        public const string Body = "Body";

        /// <summary>
        ///     SOAPAction HTTP header
        /// </summary>
        public const string SoapAction = "SOAPAction";
    }
}