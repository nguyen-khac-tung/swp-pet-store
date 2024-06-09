using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Attribute
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly PetStoreDBContext _context;

        public AttributeRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<AttributeViewModel> GetAttributes()
        {
            var attributes = _context.Attributes.Select(a => new AttributeViewModel
            {
                Id = a.AttributeId,
                Name = a.Name
            }).ToList();

            return attributes;
        }
    }
}
