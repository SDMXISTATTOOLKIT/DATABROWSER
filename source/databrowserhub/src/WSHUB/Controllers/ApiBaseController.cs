using System;
using System.Threading.Tasks;
using DataBrowser.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WSHUB.Controllers
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected readonly IMediatorService _mediatorService;

        public ApiBaseController(IMediatorService mediatorService)
        {
            _mediatorService = mediatorService ?? throw new ArgumentNullException("mediator");
        }

        protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
        {
            return await _mediatorService.Send(query);
        }

        protected async Task<TResult> UseCaseAsync<TResult>(IRequest<TResult> query)
        {
            return await _mediatorService.Send(query);
        }

        protected ActionResult<T> Single<T>(T data)
        {
            if (data == null) return NotFound();
            return Ok(data);
        }

        protected async Task<TResult> CommandAsync<TResult>(IRequest<TResult> command)
        {
            return await _mediatorService.Send(command);
        }
    }
}