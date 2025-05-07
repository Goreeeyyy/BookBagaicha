namespace BookBagaicha.IService
{
    public interface IImageService
    {
        Task<string?> SaveImageAsync(IFormFile? imageFile, Guid bookId);
        void DeleteBookImage(string? imagePath);
    }
}
