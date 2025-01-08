using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Commons.Implementations
{
    public class AWSS3Service : IAWSS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AWSS3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:Profile"]; 
        }

        public async Task<string> UploadFileAsync(IFormFile file, string keyName)
        {
            string fileName = GenerateFileName(keyName, file);

            // Mở stream từ file để upload lên S3
            using (var fileStream = file.OpenReadStream())
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fileStream,
                    Key = fileName,
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private 
                };

                using (var transferUtility = new TransferUtility(_s3Client))
                {
                    await transferUtility.UploadAsync(uploadRequest);
                }
            }

            return fileName;
        }


        public async Task<string> GetFileUrl(string keyName, int expirationMinutes = 10)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = keyName,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);

            return url;
        }

        // Phương thức xóa tệp trong S3 theo keyName
        public async Task<bool> DeleteFileAsync(string keyName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName
                };

                var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);

                // Kiểm tra phản hồi từ AWS S3 để đảm bảo xóa thành công
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        private string GenerateFileName(string keyName, IFormFile file)
        {
            var fileName = file.FileName;
            var extension = !string.IsNullOrEmpty(fileName) && fileName.Contains(".")
                ? fileName.Substring(fileName.LastIndexOf('.') + 1)
                : string.Empty;
            return $"{keyName}.{extension}";
        }

        public bool IsImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            string[] validImageMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/svg+xml" };

            return validImageMimeTypes.Contains(file.ContentType.ToLower());
        }


        public bool IsImagePathValid(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return false;
            }

            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" };
            string extension = Path.GetExtension(imagePath).ToLower();

            return validExtensions.Contains(extension);
        }

    }
}
