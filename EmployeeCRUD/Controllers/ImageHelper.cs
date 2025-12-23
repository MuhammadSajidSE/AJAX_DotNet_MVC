using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeCRUD.Controllers
{
    public static class ImageHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private const string UploadFolder = "uploads/employee-images";

        public static async Task<string?> SaveImageAsync(IFormFile imageFile, IWebHostEnvironment webHostEnvironment, int employeeId)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            // Create upload directory if it doesn't exist
            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, UploadFolder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique file name
            var fileName = $"emp_{employeeId}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return relative path for database storage
            return $"/{UploadFolder}/{fileName}";
        }

        public static void DeleteImage(string? imagePath, IWebHostEnvironment webHostEnvironment)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                var fullPath = Path.Combine(webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }
    }
}