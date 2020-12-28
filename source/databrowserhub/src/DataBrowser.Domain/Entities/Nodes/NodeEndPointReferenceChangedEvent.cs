using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Nodes
{
    public class NodeEndPointReferenceChangedEvent : INotification
    {
        public int NodeId { get; }

        public string NodeCode { get; }

        public string PreviusNodeCode { get; }

        public NodeEndPointReferenceChangedEvent(int nodeId, string nodeCode, string previusNodeCode)
        {
            NodeId = nodeId;
            NodeCode = nodeCode;
            PreviusNodeCode = previusNodeCode;
        }
    }
}
