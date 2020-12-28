namespace WSHUB.Models.Binder
{
    /*
     * 
     * 
     * EXAMPLE FOR CREATE BINDER
     * 
     * 
    [ExcludeFromCodeCoverageAttribute]
    public class NodeModelBinder : IModelBinder
    {
        const string claimName = "NodeId";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var nodeId = "";

            var userIdentity = bindingContext.HttpContext?.User?.Identity;
            if (userIdentity != null && userIdentity.IsAuthenticated)
            {
                var clamNodeId = bindingContext.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type != null && c.Type.Equals(claimName));
                nodeId = clamNodeId.Value;
            }
            if (string.IsNullOrWhiteSpace(nodeId))
            { //Take from Header only if user not autenticated or haven't nodeId in claims
                nodeId = bindingContext.HttpContext.Request.Headers[claimName].FirstOrDefault();
            }

            bindingContext.Model = new Header.NodeWs { NodeId = nodeId };
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);


            return Task.CompletedTask;
        }
    }*/
}