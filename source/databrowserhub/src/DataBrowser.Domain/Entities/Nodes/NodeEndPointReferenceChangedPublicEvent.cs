using DataBrowser.Domain.Entities.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Nodes
{
    public class NodeEndPointReferenceChangedPublicEvent : PublicEventBase
    {
        public int NodeId { get; }
        public string NodeCode { get; }
        public string PreviusNodeCode { get; }

        public NodeEndPointReferenceChangedPublicEvent(int nodeId, string nodeCode, string previusNodeCode)
        {
            NodeId = nodeId;
            NodeCode = nodeCode;
            PreviusNodeCode = previusNodeCode;
        }

    }
}
