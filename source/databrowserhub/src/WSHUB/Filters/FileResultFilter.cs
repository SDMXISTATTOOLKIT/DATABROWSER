using System;
using System.IO;
using DataBrowser.AC.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WSHUB.Filters
{
    public class FileResultFilter : IResultFilter
    {
        void IResultFilter.OnResultExecuted(ResultExecutedContext context)
        {
            try
            {
                if (context.Result is FileStreamResult)
                {
                    var fileStreamResult = (FileStreamResult) context.Result;
                    if (fileStreamResult != null && fileStreamResult.FileStream is FileStream)
                    {
                        var fileStream = (FileStream) fileStreamResult.FileStream;
                        if (fileStream.Name.ToUpperInvariant()
                                .Replace("\\", "/", StringComparison.InvariantCultureIgnoreCase)
                                .StartsWith(
                                    DataBrowserDirectory.GetTempDir().ToUpperInvariant().Replace("\\", "/") +
                                    "/download",
                                    StringComparison.InvariantCultureIgnoreCase) &&
                            File.Exists(fileStream.Name)) File.Delete(fileStream.Name);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext context)
        {
        }
    }
}