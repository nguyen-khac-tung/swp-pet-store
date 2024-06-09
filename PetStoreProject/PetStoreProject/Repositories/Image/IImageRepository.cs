namespace PetStoreProject.Repositories.Image
{
    public interface IImageRepository
    {
        public Task<string> CreateImage(string imageData);
    }
}
