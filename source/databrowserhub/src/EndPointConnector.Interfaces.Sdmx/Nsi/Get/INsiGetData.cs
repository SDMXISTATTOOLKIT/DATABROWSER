using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.Interfaces.Sdmx.Nsi.Get
{
    public interface INsiGetData
    {
        Task<GenericResponseData<IDataReaderEngine>> GetDataflowDataReaderAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria);

        Task<GenericResponseData<XmlDataContainer>> GetDataflowXmlDataAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, bool includeCodelists = true);

        Task<GenericResponseData<string>> GetDataflowJsonSdmxDataAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria);

        Task<long> GetDataflowObservationCountAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria);

        Task<GenericResponseData<string>> DownloadDataflowsAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriterian, string downloadFormat, int? maxObservations = null);

        Task<ISdmxObjects> GetDataflowWithUsedData(IDataflowObject df);
    }
}