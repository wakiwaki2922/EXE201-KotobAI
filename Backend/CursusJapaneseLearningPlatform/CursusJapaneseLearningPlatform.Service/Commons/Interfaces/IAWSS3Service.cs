using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Commons.Interfaces
{
    public interface IAWSS3Service
    {
        Task<string> UploadFileAsync(IFormFile file, string keyName);

        Task<string> GetFileUrl(string keyName, int expirationMinutes);

        Task<bool> DeleteFileAsync(string keyName);

        bool IsImagePathValid(string imagePath);

        bool IsImageFile(IFormFile file);
    }
}
