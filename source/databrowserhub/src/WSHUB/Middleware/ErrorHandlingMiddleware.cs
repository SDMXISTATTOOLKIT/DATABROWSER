using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using DataBrowser.AC.Exceptions;
using DataBrowser.Domain.Serialization;
using EndPointConnector.Interfaces.Excepetions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WSHUB.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            Exception exception = null;
            try
            {
                await _next.Invoke(context);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                exception = ex;
            }

            context.Response.ContentType = "application/json";

            var message = "";
            if (exception is AuthenticationException)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                message = DataBrowserJsonSerializer.SerializeObject(new
                {
                    errorCode = exception.Message,
                    message = ""
                });
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                message = DataBrowserJsonSerializer.SerializeObject(new
                {
                    errorCode = exception.Message,
                    message = ""
                });
            }
            else if (exception is ClientErrorException)
            {
                var clientErrorException = (ClientErrorException) exception;
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                message = DataBrowserJsonSerializer.SerializeObject(new
                {
                    errorCode = clientErrorException.ErrorCode,
                    message = clientErrorException.ErrorMessage,
                    showMessage = clientErrorException.ShowMessage
                });
            }
            else if (exception is InsufficentPermissionException)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                message = exception.Message;
            }
            else if (exception is UnauthorizedException)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                message = exception.Message;
            }
            else if (exception is ILimitDataException)
            {
                var clientErrorException = (ILimitDataException) exception;
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                message = DataBrowserJsonSerializer.SerializeObject(new
                {
                    errorCode = "LimitExceeded",
                    message = exception.Message,
                    showMessage = true
                });
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                message = DataBrowserJsonSerializer.SerializeObject(new
                {
                    errorCode = "INTERNAL_ERROR_SERVER",
                    message = ""
                });
            }

            await context.Response.WriteAsync(message);
        }
    }
}