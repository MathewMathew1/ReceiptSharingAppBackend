namespace ReceiptSharing.Api.Repositories {
    public interface IImageStorage{
        Task<string?> PostImage(IFormFile File);
    }
}