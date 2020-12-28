using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.Command.ViewTemplates.Model;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Entities.ViewTemplates.Validators;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.Domain.Validators;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.ViewTemplates
{
    public class EditViewTemplateCommand : ViewTemplatesCommandBase, ICommand<CreateOrUpdateViewTemplateResult>
    {
        public EditViewTemplateCommand(ViewTemplateDto viewTemplate,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal specificUser = null)
            : base(viewTemplate.NodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, specificUser)
        {
            ViewTemplateId = viewTemplate;
        }

        public ViewTemplateDto ViewTemplateId { get; set; }

        public class EditViewTemplateHandler : IRequestHandler<EditViewTemplateCommand, CreateOrUpdateViewTemplateResult>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IEnumerable<IRuleSpecification<ViewTemplateDto>> _rules;
            private readonly IFilterView _filterView;
            private readonly ILogger<EditViewTemplateCommand> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _repository;

            public EditViewTemplateHandler(ILogger<EditViewTemplateCommand> logger,
                IRepository<ViewTemplate> repository,
                IMapper mapper,
                IFilterView filterView,
                IFilterTemplate filterTemplate,
                IEnumerable<IRuleSpecification<ViewTemplateDto>> rules)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterView = filterView;
                _filterTemplate = filterTemplate;
                _rules = rules;
            }

            public async Task<CreateOrUpdateViewTemplateResult> Handle(EditViewTemplateCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var viewTemplateEntity = await _repository.GetByIdAsync(request.ViewTemplateId.ViewTemplateId);

                if (viewTemplateEntity == null) CreateOrUpdateViewTemplateResult.ErrorResponse("Null entity", CreateOrUpdateViewTemplateErrorType.PARAMETERS); //error handling

                var viewTemplateDto = viewTemplateEntity.ConvertToViewTemplateDto(_mapper);
                var readerPermission = ViewTemplateHelper.HavePermission(
                    filterByPermissionNodeTemplate: true,
                    filterByPermissionNodeView: true,
                    filterBySpecificNodeId: request.NodeId,
                    filterBySpecificUser: request.SpecificUser,
                    viewTemplate: viewTemplateDto,
                    filterTemplate: _filterTemplate,
                    filterView: _filterView,
                    logger: _logger);

                if (!readerPermission)
                {
                    _logger.LogDebug($"Haven't permission for current viewTemplate {request.ViewTemplateId}");
                    return CreateOrUpdateViewTemplateResult.ErrorResponse("No permission", CreateOrUpdateViewTemplateErrorType.PERMISSION);
                }

                _logger.LogDebug("update entity repository");
                var validator = await viewTemplateEntity.EditAsync(request.ViewTemplateId, _rules);

                if (!validator.IsValid)
                {
                    return processError(validator?.BrokenRules);
                }

                _logger.LogDebug("edit to repository");
                _repository.Update(viewTemplateEntity);

                _logger.LogDebug("SaveChangeAsync");
                var result = await _repository.UnitOfWork.SaveChangesAsync();

                if (result == 0) return CreateOrUpdateViewTemplateResult.ErrorResponse("Cannot update", CreateOrUpdateViewTemplateErrorType.GENERIC);

                _logger.LogDebug("END");
                return CreateOrUpdateViewTemplateResult.SuccessResponse(viewTemplateEntity.ViewTemplateId);
            }

            private CreateOrUpdateViewTemplateResult processError(IEnumerable<ValidatorResult> validatorResult)
            {
                if (validatorResult == null)
                {
                    return CreateOrUpdateViewTemplateResult.ErrorResponse("Cannot update", CreateOrUpdateViewTemplateErrorType.GENERIC);
                }

                var error = validatorResult.SelectMany(i => i.Errors).FirstOrDefault(i => i.Type == ValidatorType.Rules && i.Code.Equals(RulesConstant.ErrorCode.DuplicateName));
                if (error != null)
                {
                    return CreateOrUpdateViewTemplateResult.ErrorResponse(error?.Detail?.JsonData ?? "", CreateOrUpdateViewTemplateErrorType.TITLE_COLLISION);
                }

                return CreateOrUpdateViewTemplateResult.ErrorResponse("Cannot update", CreateOrUpdateViewTemplateErrorType.GENERIC);
            }

        }
    }
}