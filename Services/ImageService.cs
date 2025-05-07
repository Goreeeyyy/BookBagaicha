using BookBagaicha.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookBagaicha.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> SaveImageAsync(IFormFile? imageFile, Guid bookId)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Invalid image file.");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "books");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{bookId}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("images", "books", fileName).Replace("\\", "/");
        }

        public void DeleteBookImage(string? imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
    }
}
