using System;
using DataBrowser.Interfaces;

namespace DataBrowser.AC.Utility
{
    public static class RequestContextUtility
    {
        public static Tuple<int, string> GetOriginalUseCaseRequestNodeContext(IRequestContext requestContext)
        {
            return Tuple.Create(requestContext.NodeId, requestContext.NodeCode);
        }

        public static void RestoreOriginalUseCaseRequestNodeContext(IRequestContext requestContext,
            int nodeId,
            string nodeCode)
        {
            requestContext.OverwriteNodeId(nodeId);
            requestContext.OverwriteNodeCode(nodeCode);
        }
    }
}