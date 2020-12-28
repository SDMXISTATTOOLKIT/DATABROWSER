// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.EndPointConnector.EndPointConnectorFactory.Create(DataBrowser.Interfaces.EndPointConnector.IEndPointConfig)~System.Threading.Tasks.Task{DataBrowser.Interfaces.EndPointConnector.IEndPointConnector}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Middleware.TrackerAnonymousUserMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.EndPointConnector.EndPointConnectorFactory.Create(EndPointConnector.Interfaces.IEndPointConfig)~System.Threading.Tasks.Task{EndPointConnector.Interfaces.IEndPointConnector}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Middleware.TrackerAnonymousUserMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext,DataBrowser.Interfaces.IRequestContext)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Services.MediatorService.CommandAsync``1(MediatR.IRequest{``0},System.Threading.CancellationToken)~System.Threading.Tasks.Task{``0}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Services.MediatorService.Publish(System.Object,System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Services.MediatorService.Publish``1(``0,System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Services.MediatorService.QueryAsync``1(MediatR.IRequest{``0},System.Threading.CancellationToken)~System.Threading.Tasks.Task{``0}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Services.MediatorService.Send(System.Object,System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.Object}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.Services.MediatorService.Send``1(MediatR.IRequest{``0},System.Threading.CancellationToken)~System.Threading.Tasks.Task{``0}")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:DataBrowser.AC.EndPointConnector.EndPointConfig.CategorySchemaExcludes")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:DataBrowser.AC.EndPointConnector.EndPointConfig.Extras")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Adapter.DataBrowserMemoryCache.Add``1(``0,DataBrowser.Interfaces.ICacheKey{``0})~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.CustomPasswordPolicy.ValidateAsync(Microsoft.AspNetCore.Identity.UserManager{DataBrowser.Domain.Entities.Users.User},DataBrowser.Domain.Entities.Users.User,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Identity.IdentityResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.CustomUsernameEmailPolicy.ValidateAsync(Microsoft.AspNetCore.Identity.UserManager{DataBrowser.Domain.Entities.Users.User},DataBrowser.Domain.Entities.Users.User)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Identity.IdentityResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.AddNodePermissionAsync(System.Int32,System.Int32,DataBrowser.Interfaces.Constants.PermissionType.NodePermission)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.createAuthenticatedUserAsync(DataBrowser.Domain.Entities.Users.User)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.Users.UserAuthenticatedResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.DisableUser(System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.GetTokenAsync(DataBrowser.Interfaces.Dto.Users.TokenRequestDto)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.Users.UserAuthenticatedResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.RefreshTokenAsync(System.String)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.Users.UserAuthenticatedResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.RegisterAsync(DataBrowser.Interfaces.Dto.Users.UserRegisterDto)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.Users.UserRegisterResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.RemoveAllNodePermissionAsync(System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.RemoveUserRefreshToken(System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.SetNodePermissionAsync(System.Int32,System.Int32,System.Collections.Generic.List{DataBrowser.Interfaces.Constants.PermissionType.NodePermission})~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.SetRoles(System.Int32,System.Collections.Generic.List{DataBrowser.Interfaces.Constants.UserAndGroup.Roles})~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target = "~M:DataBrowser.AC.DataCache.DataflowDataCache.clearCacheAsync~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.ClearCacheJsonStatForDataflowData(System.Collections.Generic.List{System.Int32})~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.ClearSingleItemCache(System.Guid,System.Int32)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.createAnnotationStringAsync(EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{System.String}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.CreateDataflowDataCacheInfo(DataBrowser.Interfaces.Dto.DataflowDataCacheInfo)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheInfo}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.dataCacheIsStillValidAsync(EndPointConnector.Models.Dataflow,DataBrowser.Interfaces.Dto.DataflowDataCacheFile,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData(System.String)~System.Threading.Tasks.Task{System.Collections.Generic.List{DataBrowser.Interfaces.Dto.DataflowDataCacheFile}}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetCacheInfoByNodeIdAndDataflowId(System.Int32,System.String)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheInfo}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.getFirstFilterCompatibleAndValid(System.Collections.Generic.List{DataBrowser.Interfaces.Dto.DataflowDataCacheFile},System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheFile}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetInfoFromNodeId(System.Int32)~System.Threading.Tasks.Task{System.Collections.Generic.List{DataBrowser.Interfaces.Dto.DataflowDataCacheInfo}}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetJsonStatForDataflowDataFromValidKey(DataBrowser.Interfaces.Dto.UseCases.Requests.DataFromDataflowRequest,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.UseCases.Responses.GetDataFromDataflowResponse}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetJsonStatForDataflowDataFromValidKeyCompatible(DataBrowser.Interfaces.Dto.UseCases.Requests.DataFromDataflowRequest,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.UseCases.Responses.GetDataFromDataflowResponse}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetOnlyCachedKeyInfoDataflowData(System.String,System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},System.Boolean,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheFile}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetOnlyCachedKeyInfoDataflowDataIfIsValid(System.String,System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheFile}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.SetJsonStatForDataflowData(DataBrowser.Interfaces.Dto.UseCases.Requests.DataFromDataflowRequest,DataBrowser.Interfaces.Dto.UseCases.Responses.GetDataFromDataflowResponse,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.UpdateDataflowTTLFromNodeId(System.Guid,System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target = "~M:DataBrowser.AC.Workers.ApplicationCleaner.DoWorkAsync~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.DisableUser(System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.RefreshTokenAsync(System.String)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.Users.UserAuthenticatedResult}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.RemoveAllNodePermissionAsync(System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.Authentication.UserService.SetRoles(System.Int32,System.Collections.Generic.List{DataBrowser.Interfaces.Constants.UserAndGroup.Roles})~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.checkCacheEnableForCurrentDataAsync(System.String,System.Nullable{System.Boolean},System.Nullable{System.Boolean},System.Collections.Generic.List{System.String})~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target = "~M:DataBrowser.AC.DataCache.DataflowDataCache.clearCacheAsync~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.ClearSingleItemCache(System.Guid,System.Int32)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.CreateDataflowDataCacheInfo(DataBrowser.Interfaces.Dto.DataflowDataCacheInfo)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheInfo}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.getAllUnExpiredCachedKeyInfoAssociatedAtSpecificDataflowWithAllData(System.String)~System.Threading.Tasks.Task{System.Collections.Generic.List{DataBrowser.Interfaces.Dto.DataflowDataCacheFile}}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetCacheInfoByNodeIdAndDataflowId(System.Int32,System.String)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheInfo}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetInfoFromNodeId(System.Int32)~System.Threading.Tasks.Task{System.Collections.Generic.List{DataBrowser.Interfaces.Dto.DataflowDataCacheInfo}}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetJsonStatForDataflowDataFromValidKey(DataBrowser.Interfaces.Dto.UseCases.Requests.DataFromDataflowRequest,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.UseCases.Responses.GetDataFromDataflowResponse}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetJsonStatForDataflowDataFromValidKeyCompatible(DataBrowser.Interfaces.Dto.UseCases.Requests.DataFromDataflowRequest,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.UseCases.Responses.GetDataFromDataflowResponse}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.GetOnlyCachedKeyInfoDataflowData(System.String,System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},System.Boolean,EndPointConnector.Models.Dataflow,EndPointConnector.Models.Dsd)~System.Threading.Tasks.Task{DataBrowser.Interfaces.Dto.DataflowDataCacheFile}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.DataCache.DataflowDataCache.UpdateDataflowTTLFromNodeId(System.Guid,System.Int32)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.AC.EndPointConnector.EndPointConnectorFactory.Create(EndPointConnector.Interfaces.IEndPointConfig)~System.Threading.Tasks.Task{EndPointConnector.Interfaces.IEndPointConnector}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target = "~M:DataBrowser.AC.Workers.ApplicationCleaner.DoWorkAsync~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member", Target = "~P:DataBrowser.AC.Adapter.RequestContextAdapter.NodeCode")]