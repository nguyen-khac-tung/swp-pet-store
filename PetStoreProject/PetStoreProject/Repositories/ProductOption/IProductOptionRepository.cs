using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.ProductOption
{
    public interface IProductOptionRepository
    {
        public Task<string> CreateProductOption(ProductOptionCreateRequestViewModel productOptionCreateRequest, int productId, int imageId);
    }
}
