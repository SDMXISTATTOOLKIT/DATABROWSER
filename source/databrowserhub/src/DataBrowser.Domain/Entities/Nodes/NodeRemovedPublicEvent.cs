using DataBrowser.Domain.Entities.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Nodes
{
    public class NodeRemovedPublicEvent : PublicEventBase
    {
        public int NodeId { get; }

        public NodeRemovedPublicEvent(int nodeId)
        {
            NodeId = nodeId;
        }

    }
}
