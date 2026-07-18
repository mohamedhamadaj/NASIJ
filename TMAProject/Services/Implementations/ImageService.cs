using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TMAProject.Services.Interfaces;

namespace TMAProject.Services.Implementations
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;


        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<string> UploadImageAsync(
            IFormFile image,
            string folderName,
            CancellationToken cancellationToken = default)
        {

            var folderPath = Path.Combine(
                _environment.WebRootPath,
                "images",
                folderName
            );


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (image is null || image.Length == 0)
                throw new ArgumentException("No image file was provided.", nameof(image));


            var fileName = Guid.NewGuid().ToString()
                           + Path.GetExtension(image.FileName);


            var filePath = Path.Combine(folderPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream, cancellationToken);
            }


            return $"/images/{folderName}/{fileName}";
        }



        public Task DeleteImageAsync(
            string imageUrl,
            string folderName,
            CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(
                _environment.WebRootPath,
                imageUrl.TrimStart('/')
            );


            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }


            return Task.CompletedTask;
        }
    }
}