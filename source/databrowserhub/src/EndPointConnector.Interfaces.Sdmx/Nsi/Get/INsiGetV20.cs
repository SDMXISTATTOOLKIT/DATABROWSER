using System.Collections.Generic;
using System.Threading.Tasks;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;

namespace EndPointConnector.Interfaces.Sdmx.Nsi.Get
{
    public interface INsiGetV20
    {
        /// <summary>
        ///     Sends the specified <paramref name="references" /> to the Web Service defined by <see cref="_config" />
        /// </summary>
        Task<ISdmxObjects> SendQueryStructureRequestV20Async(IEnumerable<IStructureReference> references,
            bool resolveReferences);
    }
}