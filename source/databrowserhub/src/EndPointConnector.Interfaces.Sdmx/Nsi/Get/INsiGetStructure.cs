using System.Collections.Generic;
using System.Threading.Tasks;
using EndPointConnector.Models;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace EndPointConnector.Interfaces.Sdmx.Nsi.Get
{
    public interface INsiGetStructure
    {
        /// <summary>
        ///     Retrieves all available categorisations. (need V20 client inizialized)
        /// </summary>
        Task<ISdmxObjects> GetDataForTreeAsync(bool useCache = false);

        /// <summary>
        ///     Gets a bean with data about the codelist for specified dataflow and component.
        ///     The dataflow can be retrieved from <see cref="RetrieveTree" /> and the component from <see cref="GetStructure" />
        /// </summary>
        Task<ISdmxObjects> GetCodeListCostraintAsync(
            IDataflowObject dataflow,
            IDataStructureObject dsd,
            string componentId,
            bool useCache = false,
            bool orderItems = false);

        /// <summary>
        ///     Gets a bean with data about the codelist for specified dataflow and component filtered.
        /// </summary>
        Task<ISdmxObjects> GetCodeListCostraintFilterAsync(
            IDataflowObject dataflow,
            IDataStructureObject dsd,
            string criteriaId,
            List<FilterCriteria> filterComponents,
            bool useCache = false,
            bool orderItems = false);

        /// <summary>
        ///     Retrieves all available categorisations and category schemes.
        /// </summary>
        Task<ISdmxObjects> GetCategorySchemesAndCategorisationsAsync(bool useCache = false);

        /// <summary>
        ///     Retrieves all available dataflows.
        /// </summary>
        Task<ISdmxObjects> GetDataflowsAsync(bool useCache = false);

        Task<ISdmxObjects> GetOnlyDataflowsValidForCatalogWithDsdAndCodelistAsync(bool useCache = false);
    }
}