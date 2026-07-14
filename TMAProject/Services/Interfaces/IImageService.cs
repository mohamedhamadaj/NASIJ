namespace TMAProject.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile image, string folderName, CancellationToken cancellationToken = default);

        Task DeleteImageAsync(string imageUrl,string folderName, CancellationToken cancellationToken = default);
    }
}
