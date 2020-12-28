using System.Threading.Tasks;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;

namespace EndPointConnector.Interfaces.Sdmx.Nsi.Get
{
    public interface INsiGetArtefact
    {
        /// <summary>
        ///     Retrieves the specific artefact.
        /// </summary>
        Task<ISdmxObjects> GetArtefactAsync(SdmxStructureEnumType type, string id, string agency, string version,
            StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None, string respDetail = "",
            bool useCache = false, bool includeCrossReference = true, bool orderItems = false);
    }
}