using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public class FileUploader
    {
        // Should be in appsettings.json but I put in here for easy tracking and readability 
        private const string FilePath = @"C:\YourMediaFilePath";
        private static string FileUrl = "http://YourMediaFileUrlToAccess";

        public static List<string> UploadFilesWithFolder(List<IFormFile> files, string folderName)
        {
            #region Extension validation
            //string[] permittedExtension = { ".txt", ".pdf", "xlsx" };
            //var ext = Path.GetExtension(uploadedFileName).ToLowerInvariant();
            //if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            //{
            //    // The extension is invalid 
            //}
            #endregion

            var copiedFileUrls = new List<string>();

            // If the folder does not exist yet, it will be created.
            // If the folder exists already, the line will be ignored.
            var createdDirectory = Directory.CreateDirectory(Path.Combine(FilePath, folderName));
            var dirname = createdDirectory.FullName;

            foreach (var formFile in files)
            {
                var fileName = formFile.FileName;
                var fullUrl = Path.Combine(dirname, fileName); // MyDirectory/MyFile.docx
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(FileUrl, folderName, fileName)/*.Replace("\\", @"\")*/;     // http://YourMediaFileUrlToAccess/MyDirectory/MyFile.docx
                    using (var stream = File.Create(fullUrl))
                    {
                        formFile.CopyTo(stream);
                    }
                    copiedFileUrls.Add(filePath);
                }
            }
            return copiedFileUrls;
        }
    }
}
