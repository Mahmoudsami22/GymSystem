using GymSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Services.Classes
{
    public class AttachementServices : IAttachementServices
    {
        public AttachementServices(ILogger<AttachementServices> logger,IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }
        private readonly long maxFileSize = 5 * 1024 * 1024; //5MB
        private readonly string[] allowedExtentions = { ".jpg", ". jpeg", ".png" };
        private readonly ILogger<AttachementServices> logger;
        private readonly IWebHostEnvironment env;

        public bool Delete(string fileName, string FolderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(FolderName)) return false;

            try
            {
                var fullPath = Path.Combine(env.ContentRootPath, FolderName, fileName);

                if (!File.Exists(fullPath)) return false;

                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Delete the Attachment");
                return false;


            }
        }

        public (Stream stream, string contentType)? GetFile(string fileName, string FolderName)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead) return null;

            if (fileStream.Length == 0) return null;

            if (fileStream.Length > maxFileSize)
            {
                logger.LogWarning("Rejected File Too Large");
                return null;
            }
            var extention = Path.GetExtension(fileName);//.Ay 7aga

            if (string.IsNullOrEmpty(extention) || !allowedExtentions.Contains(extention))
            {
                logger.LogWarning("Reject Wrong Extention File");
                return null;
            }
            var UploadedFolder = Path.Combine(env.ContentRootPath, folderName);

            Directory.CreateDirectory(UploadedFolder);

            var storedFileName = $"{Guid.NewGuid()} {extention}";//.p

            var filePath = Path.Combine(UploadedFolder, storedFileName);//Full Path

            try
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

                await fileStream.CopyToAsync(fs);
                return storedFileName;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Upload file");
                return null;
            }
        } 
    }
}
