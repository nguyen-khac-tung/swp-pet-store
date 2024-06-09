using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Attribute
{
    public interface IAttributeRepository
    {
        public List<AttributeViewModel> GetAttributes();
    }
}
