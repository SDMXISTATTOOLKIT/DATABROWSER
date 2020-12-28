using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WSHUB.Models.Request;
using WSHUB.Models.Response;
using WSHUB.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace WSHUB.Controllers
{
    [ApiController]
    public class GenericController : ApiBaseController
    {

        public GenericController(ILogger<GenericController> logger,
            IMediatorService mediatorService)
            : base(mediatorService)
        {
        }

        /// <summary>
        ///     Create new node.
        /// </summary>
        /// <response code="200">File uploaded</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("File/Upload/{subdir?}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.UploadFile)]
        public async Task<ActionResult> Upload(IFormFile[] files, string subdir = null)
        {
            var filenames = new List<string>();
            if (files == null || files.Length == 0)
            {
                var resultNoData = new ContentResult();
                resultNoData.ContentType = "application/text";
                resultNoData.Content = "No file to upload";
                resultNoData.StatusCode = 400;
                return resultNoData;
            }


            if (!string.IsNullOrWhiteSpace(subdir))
                subdir = string.Concat(subdir.Split(Path.GetInvalidPathChars()))
                    .Replace("..\\", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace(".\\", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace(".//", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("\\", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("//", "", StringComparison.InvariantCultureIgnoreCase);

            try
            {
                foreach (var file in files)
                {
                    var allow = DataBrowserDirectory.AllowedFileFormat(file);
                    if (!allow)
                        continue;

                    var fileName = Path.GetFileName(file.FileName);
                    fileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
                    var folder = DataBrowserDirectory.GetUploadPath(fileName);
                    if (!string.IsNullOrWhiteSpace(subdir)) folder = Path.Combine(folder, subdir);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);

                    var i = 1;
                    while (System.IO.File.Exists(filePath))
                    {
                        var fileNameTmp = Path.GetFileNameWithoutExtension(fileName) + $"({i})" +
                                          Path.GetExtension(fileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), folder, fileNameTmp);
                        i++;
                    }

                    using (var localFile = System.IO.File.OpenWrite(filePath))
                    {
                        using (var uploadedFile = file.OpenReadStream())
                        {
                            await uploadedFile.CopyToAsync(localFile);
                        }
                    }

                    filenames.Add(DataBrowserDirectory.ConvertAbsoluteToRelativePath(filePath));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"UploadError: {ex.Message}", ex);
            }

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(filenames);
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get DataBrowser version
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <response code="200">DataBrowser version</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Version")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Version))]
        public async Task<ActionResult> GetNode(int nodeId)
        {
            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = VersionDataBrowser.Current.ToString();
            result.StatusCode = 200;
            return result;
        }
    }
}