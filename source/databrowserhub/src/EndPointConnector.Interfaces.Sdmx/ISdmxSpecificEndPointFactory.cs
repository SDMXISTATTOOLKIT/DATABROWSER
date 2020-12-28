using System.Threading.Tasks;
using EndPointConnector.Interfaces.Sdmx.Dm;
using EndPointConnector.Interfaces.Sdmx.Ma;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;

namespace EndPointConnector.Interfaces.Sdmx
{
    public interface ISdmxSpecificEndPointFactory
    {
        /// <summary>
        ///     Create an EndPoint NSI connector fully compatible with V2.1
        /// </summary>
        /// <param name="nodeWS">
        ///     Configuration settings
        /// </param>
        /// <param name="endPointType">
        ///     Inizialize endpoint type (SOAP SDMX V2.1, REST SDMX) or read the type from Configuration settings
        ///     <paramref name="nodeWS" />
        /// </param>
        /// <param name="inizializeSoapV20">
        ///     In case of <paramref name="endPointType" /> SOAP V2.1 or REST SDMX encapsulate and inizialize the SOAP SDMX V2.0
        ///     client (used only for some request)
        /// </param>
        /// <returns>
        ///     The EndPoint NSI connector.
        /// </returns>
        Task<INsiConnector> CreateSdmxConnectorAsync(EndPointConfig endPointConfig,
            SdmxEndPointCostant.ConnectorType endPointType = SdmxEndPointCostant.ConnectorType.FromConfig,
            bool inizializeSoapV20 = false);

        IDmConnector CreateDmConnector(EndPointConfig endPointConfig);

        IMaConnector CreateMaConnector(EndPointConfig endPointConfig);
    }
}