using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataBrowser.AC.Utility
{
    public class DataBrowserDirectory
    {
        public enum TempFileType
        {
            Download,
            Generic
        }

        private const string folderStorage = "wwwroot";
        private const string folderTmpData = "_StorageTmpDataContainer";

        private static readonly Dictionary<string, string> fileToDirectory = new Dictionary<string, string>
        {
            {"image", "images"},
            {"video", "movies"},
            {"file", "files"}
        };

        private static readonly List<string> allowedForupload = new List<string>
        {
            {"image/jpeg"},
            {"image/png"},
            {"image/gif"},
            {"image/svg+xml"},
            {"video/mp4"}
        };


        public static string GetUploadPath(string filename)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(filename, out contentType);

            return getUploadPath(contentType);
        }

        public static string GetUploadPath(IFormFile formFile)
        {
            var contentType = formFile?.ContentType ?? "";

            return getUploadPath(contentType);
        }

        private static string getUploadPath(string contentType)
        {
            if (contentType.StartsWith("image/"))
            {
                return $"{folderStorage}\\{fileToDirectory["image"]}";
            }
            else if (contentType.StartsWith("video/"))
            {
                return $"{folderStorage}\\{fileToDirectory["video"]}";
            }
            else
            {
                return $"{folderStorage}\\{fileToDirectory["file"]}";
            }
        }

        public static bool AllowedFileFormat(IFormFile formFile)
        {
            var contentType = formFile?.ContentType ?? "";
            return allowedForupload.Any(i => i.Equals(contentType));
        }

        public static bool AllowedContentType(string contentType)
        {
            return allowedForupload.Any(i => i.Equals(contentType));
        }

        public static string GetImageDirPath()
        {
            return getDirPatch("image");
        }

        public static string GetVideoDirPath()
        {
            return getDirPatch("video");
        }

        public static string GetFileDirPath()
        {
            return getDirPatch("file");
        }

        public static string GetCategoryImageDirPath()
        {
            return Path.Combine(getDirPatch("image"), "categories");
        }

        private static string getDirPatch(string type)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $"{folderStorage}\\{fileToDirectory[type]}");
        }
        public static string GetRootStorageFolder()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $"{folderStorage}");
        }


        public static string ConvertAbsoluteToRelativePath(string path)
        {
            var tmpStr = path.Replace(Path.Combine(Directory.GetCurrentDirectory(), $"{folderStorage}\\"), "");
            tmpStr = tmpStr.Replace(Path.Combine(Directory.GetCurrentDirectory(), $"{folderTmpData}\\"), "");
            return tmpStr.Replace("\\", "/");
        }

        public static string GetTempDir()
        {
            var tempSubDir = $"{folderTmpData}\\files\\temp";
            return Path.Combine(Directory.GetCurrentDirectory(), tempSubDir);
        }

        public static string GetTempFileName(TempFileType tempFileType)
        {
            var tempSubDir = GetTempDir();
            if (!Directory.Exists(tempSubDir)) Directory.CreateDirectory(tempSubDir);

            var prefixFile = "";
            if (tempFileType == TempFileType.Download) prefixFile = "download";

            var filePath = $"{tempSubDir}\\{prefixFile}{Guid.NewGuid()}.tmp";
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.SetLength(0);
            }

            return filePath;
        }
    }
}