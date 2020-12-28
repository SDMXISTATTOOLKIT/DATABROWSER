using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Dto.UseCases.Requests
{
    public class ArtefactRequestRequest : IUseCase<GetArtefactResponse>
    {
        public ArtefactRequestRequest(ArtefactType.ArtefactEnumType type, string id,
            ArtefactType.ReferenceDetailEnumType refDetail = ArtefactType.ReferenceDetailEnumType.None,
            ArtefactType.ResponseDetailEnumType respDetail = ArtefactType.ResponseDetailEnumType.Null)
        {
            Type = type;
            Id = id;
            RefDetail = refDetail;
            RespDetail = respDetail;
        }

        public ArtefactType.ArtefactEnumType Type { get; }
        public string Id { get; }
        public ArtefactType.ReferenceDetailEnumType RefDetail { get; }
        public ArtefactType.ResponseDetailEnumType RespDetail { get; }
    }
}