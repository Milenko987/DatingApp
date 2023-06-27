namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<Result> UploadPhoto(IFormFile file);

        ResultDelete DeletePhoto(string photoId);
    }
}
