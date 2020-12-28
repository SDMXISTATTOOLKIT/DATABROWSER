// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Controllers.ArtefactController.CountItem(System.String,System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.ICountItemArtefactSdmxUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Controllers.ArtefactController.Get(System.String,System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.Services.IGetArtefactSdmxService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]

[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.API.Controllers.ArtefactController.CountItem(System.String,System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.ICountItemArtefactSdmxUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.API.Controllers.ArtefactController.Get(System.String,System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.Services.IGetArtefactSdmxService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:DataBrowser.API.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment)")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Controllers.ArtefactController.CountItem(System.String,System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.ICountItemArtefactSdmxUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Controllers.ArtefactController.Get(System.String,System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.Services.IGetArtefactSdmxService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Presenters.GenericJsonPresenter.Handle(DataBrowser.AC.Interfaces.Dto.GenericJsonResponse)")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Presenters.SdmxArtefactPresenter.Handle(DataBrowser.AC.Interfaces.Dto.ServiceResponses.GetArtefactSdmxResponse)")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:DataBrowser.API.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment)")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.CodelistController.Get(System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.IGetDataFromDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetPartialCodeList(DataBrowser.AC.Interfaces.UseCases.IGetPartialCodelistForDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetTree(DataBrowser.AC.Interfaces.UseCases.IGetTreeCategoriesUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.GetEndPointConfig(DataBrowser.Interfaces.Services.INodeService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HealthChecks.ApiHealthCheck.CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HealthChecks.HomePageHealthCheck.CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HealthChecks.PingHealthCheck.CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HealthChecks.SystemMemoryHealthcheck.CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Middleware.ErrorHandlingMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment)")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.CodelistController.Get(System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.IGetDataFromDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetPartialCodeList(DataBrowser.AC.Interfaces.UseCases.IGetPartialCodelistForDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetTree(DataBrowser.AC.Interfaces.UseCases.IGetTreeCategoriesUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.GetEndPointConfig(DataBrowser.Interfaces.Services.INodeService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.CodelistController.Get(System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.IGetDataFromDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetPartialCodeList(DataBrowser.AC.Interfaces.UseCases.IGetPartialCodelistForDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetTree(DataBrowser.AC.Interfaces.UseCases.IGetTreeCategoriesUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.GetEndPointConfig(DataBrowser.Interfaces.Services.INodeService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target = "~M:WSHUB.Presenters.GenericJsonPresenter.Handle(DataBrowser.AC.Interfaces.Dto.GenericJsonResponse)")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Presenters.SdmxArtefactPresenter.Handle(DataBrowser.Interfaces.EndPointConnector.Sdmx.Models.SdmxResponse)")]
[assembly:
    SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment)")]
[assembly:
    SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.CodelistController.Get(System.String,System.String,System.String,System.String,DataBrowser.AC.Interfaces.UseCases.IGetDataFromDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetPartialCodeList(DataBrowser.AC.Interfaces.UseCases.IGetPartialCodelistForDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Middleware.ErrorHandlingMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.CodelistController.Get(System.String,System.String,System.String,System.String,DataBrowser.Interfaces.UseCases.IGetDataFromDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetController.GetCriteria(DataBrowser.Interfaces.UseCases.IGetCriteriaForDataflowUseCase,System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetController.GetCriteriaFilter(DataBrowser.Interfaces.UseCases.IGetCriteriaFilterForDataflowUseCase,System.Collections.Generic.Dictionary{System.String,System.Collections.Generic.List{System.String}},System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetController.GetFull(DataBrowser.Interfaces.UseCases.IGetFullCodelistForDataflowUseCase,System.String,System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetController.GetPartial(DataBrowser.Interfaces.UseCases.IGetPartialCodelistForDataflowUseCase,System.String,System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetPartialCodeList(DataBrowser.Interfaces.UseCases.IGetPartialCodelistForDataflowUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeneralController.GetTree(DataBrowser.Interfaces.UseCases.IGetTreeCategoriesUseCase)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetAllHubs(DataBrowser.Interfaces.Services.ICrud{DataBrowser.Interfaces.Dto.Services.HubDto,DataBrowser.Interfaces.Dto.Services.HubDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetNode(DataBrowser.Interfaces.Services.ICrud{DataBrowser.Interfaces.Dto.Services.HubDto,DataBrowser.Interfaces.Dto.Services.HubDto},System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.HubPatch(DataBrowser.Interfaces.Services.ICrud{DataBrowser.Interfaces.Dto.Services.HubDto,DataBrowser.Interfaces.Dto.Services.HubDto},System.Int32,Microsoft.AspNetCore.JsonPatch.JsonPatchDocument{DataBrowser.Interfaces.Dto.Services.HubDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.UpdateNode(DataBrowser.Interfaces.Services.ICrud{DataBrowser.Interfaces.Dto.Services.HubDto,DataBrowser.Interfaces.Dto.Services.HubDto},DataBrowser.Interfaces.Dto.Services.HubDto)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.CreateNode(DataBrowser.Interfaces.Services.INodeService,WSHUB.Models.Request.NodeCreateRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.DeleteNode(DataBrowser.Interfaces.Services.INodeService,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.GetAllNode(DataBrowser.Interfaces.Services.INodeService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.GetNode(DataBrowser.Interfaces.Services.INodeService,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.NodePatch(DataBrowser.Interfaces.Services.INodeService,System.Int32,Microsoft.AspNetCore.JsonPatch.JsonPatchDocument{DataBrowser.Interfaces.Dto.Services.NodeDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodeController.UpdateNode(DataBrowser.Interfaces.Services.INodeService,WSHUB.Models.Request.NodeUpdateRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ApiBaseController.CommandAsync``1(MediatR.IRequest{``0})~System.Threading.Tasks.Task{``0}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ApiBaseController.QueryAsync``1(MediatR.IRequest{``0})~System.Threading.Tasks.Task{``0}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetConstraintFilter(System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetCountObservationsFromDataflowUseCase(System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetCountObservationsWithFilterFromDataflowUseCase(System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetDataFromDataflowUseCase(System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetDataWithFilterFromDataflowUseCase(System.Collections.Generic.List{EndPointConnector.Models.FilterCriteria},System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetFull(System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetPartial(System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetStructure(System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetAllHubs(DataBrowser.Services.Interfaces.ICrud{DataBrowser.Domain.Dtos.HubDto,DataBrowser.Domain.Dtos.HubDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetAllHubsConfig(DataBrowser.Services.Interfaces.ICrud{DataBrowser.Domain.Dtos.HubDto,DataBrowser.Domain.Dtos.HubDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetHub(DataBrowser.Services.Interfaces.ICrud{DataBrowser.Domain.Dtos.HubDto,DataBrowser.Domain.Dtos.HubDto},System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetNodeWithMinimalInfo(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.HubPatch(DataBrowser.Services.Interfaces.ICrud{DataBrowser.Domain.Dtos.HubDto,DataBrowser.Domain.Dtos.HubDto},System.Int32,Microsoft.AspNetCore.JsonPatch.JsonPatchDocument{DataBrowser.Domain.Dtos.HubDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.UpdateHub(DataBrowser.Services.Interfaces.ICrud{DataBrowser.Domain.Dtos.HubDto,DataBrowser.Domain.Dtos.HubDto},DataBrowser.Domain.Dtos.HubDto)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.CreateNode(DataBrowser.Services.Interfaces.INodeService,WSHUB.Models.Request.NodeCreateRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.DeleteNode(DataBrowser.Services.Interfaces.INodeService,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetAllNode(DataBrowser.Services.Interfaces.INodeService)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetNode(DataBrowser.Services.Interfaces.INodeService,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target = "~M:WSHUB.Controllers.NodesController.getNodeCatalogAsync~System.Threading.Tasks.Task{System.String}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetNodeDataView(DataBrowser.Services.Interfaces.INodeService,DataBrowser.Services.Interfaces.ICrud{DataBrowser.Domain.Dtos.HubDto,DataBrowser.Domain.Dtos.HubDto},System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetTree~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.OrderNodes(DataBrowser.Services.Interfaces.INodeService,System.Collections.Generic.List{System.Int32})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.PatchNode(DataBrowser.Services.Interfaces.INodeService,System.Int32,Microsoft.AspNetCore.JsonPatch.JsonPatchDocument{DataBrowser.Domain.Dtos.NodeDto})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.UpdateNode(DataBrowser.Services.Interfaces.INodeService,WSHUB.Models.Request.NodeUpdateRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ApiBaseController.UseCaseAsync``1(MediatR.IRequest{``0})~System.Threading.Tasks.Task{``0}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetAllHubs~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetAllHubsConfig~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.GetHub(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.HubController.UpdateHub(DataBrowser.Domain.Dtos.HubDto)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.CreateNode(WSHUB.Models.Request.NodeCreateRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.DeleteNode(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetAllNode~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetNode(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetNodeDataView(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetQueryTest~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetQueryTest2~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.TestEdit~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.TestEdit2~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.TestInsert~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.UpdateNode(WSHUB.Models.Request.NodeUpdateRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.TracingController.GetTracing(System.String,System.Nullable{System.Boolean})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.TracingController.GetTracingFilter(System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Request.GetCriteriaFilter.Filters")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CategoryGroupModelView.Categories")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CategoryGroupModelView.Extras")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CategoryModelView.ChildrenCategories")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CategoryModelView.DatasetIdentifiers")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CategoryModelView.Extras")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CriteriaViewMode.Extra")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.CriteriaViewMode.Values")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetCriteriaViewModel.Criteria")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetCriteriaViewModel.GeoIds")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetCriteriaViewModel.Keywords")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetCriteriaViewModel.TerritorialDimensions")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetModelView.AttachedDataFiles")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetModelView.Extras")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetModelView.Keywords")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.DatasetModelView.LayoutFilter")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.Filters.FilterData")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.HubInfoMinimalResponse.MinimalHub.SupportedLanguages")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.HubInfoMinimalResponse.Nodes")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.HubMinimalModelView.Extras")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.HubMinimalModelView.SupportedLanguages")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.NodeModelView.CategorySchemaExcludes")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.NodeModelView.Extras")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.NodeModelView.LabelDimensionTemporals")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.NodeModelView.LabelDimensionTerritorials")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.StructureLayout.Cols")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.StructureLayout.Rows")]
[assembly:
    SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>",
        Scope = "member", Target = "~P:WSHUB.Models.Response.StructureLayout.Sections")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.DownloadDataflow(System.String,System.String,System.String,System.String,System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.DataSetsController.GetStructure(System.String,System.String,System.Nullable{System.Int32})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.ChangeTTLDataflowDataCache(System.Int32,System.Guid,DataBrowser.Interfaces.IDataflowDataCache,WSHUB.Models.Request.UpdateDataflowDataCacheInfoRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.ClearDataCache(DataBrowser.Interfaces.IDataflowDataCache)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.ClearDataCache(System.Guid,DataBrowser.Interfaces.IDataflowDataCache)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.ClearMemoryCache(DataBrowser.Interfaces.IDataBrowserMemoryCache)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.ClearMemoryCacheNodeCatalogTree(DataBrowser.Interfaces.IDataBrowserMemoryCache,System.Nullable{System.Int32})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.CreateDataflowDataCacheInfo(System.Int32,DataBrowser.Interfaces.IDataflowDataCache,DataBrowser.Interfaces.Dto.DataflowDataCacheInfo)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.GetAllDataflowDataCacheInfoNode(System.Int32,DataBrowser.Interfaces.IDataflowDataCache)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GenericController.Upload(Microsoft.AspNetCore.Http.IFormFile[],System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeometryController.CountGeometries~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeometryController.GetAllGeometries~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeometryController.GetGeometriesById(System.Collections.Generic.List{System.String})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.GeometryController.GetSingleGeometryById(System.String)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetAllNodeConfig~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.OrderNodes(System.Collections.Generic.List{System.Int32})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.UpdateNode(WSHUB.Models.Request.NodeUpdateRequest,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.TracingController.GetTracingLast(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.BlacklistRefreshToken(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.DisableUser(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.GetTokenAsync(WSHUB.Models.Request.TokenRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.RefreshToken~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.RegisterAsync(WSHUB.Models.Request.UserRegisterRequest)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.SetPermissions(System.Int32,System.Int32,System.Collections.Generic.List{DataBrowser.Interfaces.Constants.PermissionType.NodePermission})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.UsersController.SetRoles(System.Int32,System.Collections.Generic.List{DataBrowser.Interfaces.Constants.UserAndGroup.Roles})~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.IActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.CreateTemplate(DataBrowser.Domain.Dtos.ViewTemplateDto)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.CreateView(DataBrowser.Domain.Dtos.ViewTemplateDto)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.createViewTemplateAsync(DataBrowser.Domain.Dtos.ViewTemplateDto)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ContentResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.DeleteView(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.DeleteviewTemplate(System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.GetTemplateById(System.Int32,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.GetTemplateListByNodeId~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.GetViewById(System.Int32,System.Int32)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Controllers.ViewTemplateController.GetViewListByNodeId~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.ApplicationCleanerHostedService.DoWork(System.Object)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.ApplicationCleanerHostedService.StartAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.MigratorDBHostedService.StartAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
        Justification = "<Pending>", Scope = "member",
        Target =
            "~M:WSHUB.Utils.GetTreeHelper.GetCatalogTreeAsync(DataBrowser.Services.Interfaces.IMediatorService,DataBrowser.Interfaces.IDataBrowserMemoryCache,DataBrowser.Interfaces.IRequestContext)~System.Threading.Tasks.Task{System.String}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.Controllers.NodesController.GetTree~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult}")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member", Target = "~M:WSHUB.Controllers.UsersController.setRefreshTokenInCookie(System.String)")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.ApplicationCleanerHostedService.DoWork(System.Object)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.ApplicationCleanerHostedService.StartAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.ApplicationCleanerHostedService.StopAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly:
    SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:WSHUB.HostedService.Workers.MigratorDBHostedService.StartAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task")]