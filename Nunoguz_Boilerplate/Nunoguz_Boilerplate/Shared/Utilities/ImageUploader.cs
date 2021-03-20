using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public static class ImageUploader
    {
        private const string FilePath = @"C:\YourMediaFilePath";
        private static string FileUrl = "http://YourMediaFileUrlToAccess";

        public static string UploadFileWithBase64(string base64)
        {
            var guidKey = Guid.NewGuid().ToString() + ".jpg";
            var fPath = Path.Combine(FilePath, guidKey).Replace("\\", @"\");

            var imageBytes = Convert.FromBase64String(base64);
            System.IO.File.WriteAllBytes(fPath, imageBytes);  // Throws Error, couldn't find path

            //create thumbnail
            ResizeAndSaveImage(fPath, false, true);

            return Path.Combine(FileUrl, $"{guidKey}").Replace("\\", "/");
        }

        public static List<string> UploadFilesList(List<string> base64)
        {
            var createdUrls = new List<string>();
            foreach (var item in base64)
            {
                var guidKey = Guid.NewGuid().ToString() + ".jpg";
                var fPath = Path.Combine(FilePath, guidKey);

                var imageBytes = Convert.FromBase64String(item);
                System.IO.File.WriteAllBytes(fPath, imageBytes);
                createdUrls.Add(Path.Combine(FileUrl, $"{guidKey}").Replace("\\", "/").Replace("//", "/"));
            }
            return createdUrls;
        }

        public static bool ResizeAndSaveImage(string filePath, bool isWebUrl, bool overWrite)
        {
            //C:\YourMediaFilePath\ce8cdcd1-d36e-442b-bda0-837300b095ad.jpg
            if (isWebUrl)
            {
                // convert to file path
                filePath = filePath.Replace(FileUrl, FilePath);
                filePath = filePath.Replace("\\", "/").Replace("//", "/");
            }
            // Image<Rgba32> --> var
            try
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(filePath, attributes);
                }

                using (var image = Image.Load(filePath))
                {
                    image.Mutate(x => x
                         .Resize(image.Width / 3, image.Height / 3));

                    var filename = Path.GetFileNameWithoutExtension(filePath);
                    var extension = Path.GetExtension(filePath);

                    filename = filename + "_thumb" + extension;
                    var filenameWithPath = Path.Combine(FilePath, filename);
                    var encoder = new JpegEncoder()
                    {
                        Quality = 70 //Use variable based on your requirements
                    };
                    if ((overWrite && File.Exists(filenameWithPath)) || !File.Exists(filenameWithPath))
                    {
                        image.Save(filenameWithPath, encoder); // Automatic encoder selected based on extension.
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                throw new ApiException(new Error { Message = "An error occured while resizing and uploading image to server", StackTrace = exception.Message });
            }
        }

    }
}
