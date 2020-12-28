using AutoMapper;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.Command.ViewTemplates.Model;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Entities.ViewTemplates.Validators;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.Domain.Validators;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DataBrowser.Command.ViewTemplates
{
    public class CreateViewTemplateCommand : ViewTemplatesCommandBase, ICommand<CreateOrUpdateViewTemplateResult>
    {
        public CreateViewTemplateCommand(ViewTemplateDto viewTemplate,
            int nodeId,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal specificUser = null)
            : base(nodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, specificUser)
        {
            if (UtilitySecurity.GetUserId(specificUser) <= 0)
            {
                throw new ArgumentNullException("UserId");
            }

            ViewTemplate = viewTemplate;
        }

        public ViewTemplateDto ViewTemplate { get; set; }

        public class CreateViewTemplateHandler : IRequestHandler<CreateViewTemplateCommand, CreateOrUpdateViewTemplateResult>
        {
            private readonly ILogger<CreateViewTemplateHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IEnumerable<IRuleSpecification<ViewTemplateDto>> _rules;
            private readonly IMediator _mediator;
            private readonly IRepository<ViewTemplate> _repository;

            public CreateViewTemplateHandler(ILogger<CreateViewTemplateHandler> logger,
                                            IRepository<ViewTemplate> repository,
                                            IMediator mediator,
                                            IMapper mapper,
                                            IEnumerable<IRuleSpecification<ViewTemplateDto>> rules)
            {
                _logger = logger;
                _repository = repository;
                _mediator = mediator;
                _mapper = mapper;
                _rules = rules;
            }

            public async Task<CreateOrUpdateViewTemplateResult> Handle(CreateViewTemplateCommand request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");
                request.ViewTemplate.UserId = UtilitySecurity.GetUserId(request.SpecificUser);
                ViewTemplate existingViewTemplate = await findExistingViewTemplate(request);

                if (existingViewTemplate != null) //update needed
                {
                    return await _mediator.Send(new EditViewTemplateCommand(request.ViewTemplate,
                                                                            specificUser: request.SpecificUser));
                }

                var validator = await Domain.Entities.ViewTemplates.ViewTemplate.CreateViewTemplateAsync(request.ViewTemplate, _rules);

                if (!validator.IsValid)
                {
                    return processError(validator?.BrokenRules);
                }

                var result = false;
                if (validator?.ValidateObject != null)
                {
                    _logger.LogDebug("add to repository");
                    _repository.Add(validator.ValidateObject);

                    _logger.LogDebug("SaveChangeAsync");
                    result = await _repository.UnitOfWork.SaveChangesAsync() > 0;
                }

                if (!result)
                {
                    return CreateOrUpdateViewTemplateResult.ErrorResponse("Cannot create", CreateOrUpdateViewTemplateErrorType.GENERIC);
                }

                return CreateOrUpdateViewTemplateResult.SuccessResponse(validator.ValidateObject.ViewTemplateId);
            }

            private async Task<ViewTemplate> findExistingViewTemplate(CreateViewTemplateCommand request)
            {
                var existingEntities = await _repository.FindAsync(new ViewTemplateByType_Dataset_NodeIdSpecification(
                                        request.NodeId,
                                        request.ViewTemplate.DatasetId,
                                        request.ViewTemplate.Type));
                if (request.ViewTemplate.Type == ViewTemplateType.View)
                {
                    return existingEntities?.Where(x => x.ViewTemplateId == request.ViewTemplate.ViewTemplateId).FirstOrDefault();
                }
                return existingEntities?.FirstOrDefault();
            }

            private CreateOrUpdateViewTemplateResult processError(IEnumerable<ValidatorResult> validatorResult)
            {
                if (validatorResult == null)
                {
                    return CreateOrUpdateViewTemplateResult.ErrorResponse("Cannot create", CreateOrUpdateViewTemplateErrorType.GENERIC);
                }

                var error = validatorResult.SelectMany(i => i.Errors).FirstOrDefault(i => i.Type == ValidatorType.Rules && i.Code.Equals(RulesConstant.ErrorCode.DuplicateName));
                if (error != null)
                {
                    return CreateOrUpdateViewTemplateResult.ErrorResponse(error?.Detail?.JsonData ?? "", CreateOrUpdateViewTemplateErrorType.TITLE_COLLISION);
                }

                return CreateOrUpdateViewTemplateResult.ErrorResponse("Cannot create", CreateOrUpdateViewTemplateErrorType.GENERIC);
            }
        }
    }
}