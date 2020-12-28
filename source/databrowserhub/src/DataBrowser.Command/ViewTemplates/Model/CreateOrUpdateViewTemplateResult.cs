using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Command.ViewTemplates.Model
{
    public enum CreateOrUpdateViewTemplateErrorType
    {
        NONE,
        GENERIC,
        PERMISSION,
        TITLE_COLLISION,
        INACTIVE_NODE,
        PARAMETERS
    }

    public class CreateOrUpdateViewTemplateResult
    {
        public int Id {get; set;}
        public CreateOrUpdateViewTemplateErrorType ErrorType{ get; internal set; }
        public bool HasErrors { get => ErrorType != CreateOrUpdateViewTemplateErrorType.NONE; }

        public List<string> Errors;

        protected CreateOrUpdateViewTemplateResult()
        {
            Errors = new List<string>();
            ErrorType = CreateOrUpdateViewTemplateErrorType.NONE;
        }


        public static CreateOrUpdateViewTemplateResult SuccessResponse(int id)
        {
            var result = new CreateOrUpdateViewTemplateResult();
            result.Id = id;
            result.ErrorType = CreateOrUpdateViewTemplateErrorType.NONE;
            return result;
        }

        public static CreateOrUpdateViewTemplateResult ErrorResponse(string errorMessage, CreateOrUpdateViewTemplateErrorType errorType)
        {
            var result = new CreateOrUpdateViewTemplateResult();
            result.Id = -1;
            result.ErrorType = errorType == CreateOrUpdateViewTemplateErrorType.NONE ? CreateOrUpdateViewTemplateErrorType.GENERIC : errorType;
            if (errorMessage != null)
            {
                result.Errors.Add(errorMessage);

            }            
            return result;
        }

        public static CreateOrUpdateViewTemplateResult ErrorResponse(List<string> errorMessages, CreateOrUpdateViewTemplateErrorType errorType)
        {
            var result = new CreateOrUpdateViewTemplateResult();
            result.Id = -1;
            result.ErrorType = errorType == CreateOrUpdateViewTemplateErrorType.NONE ? CreateOrUpdateViewTemplateErrorType.GENERIC : errorType;
            result.Errors = errorMessages ?? result.Errors;
            return result;
        }

    }
}
