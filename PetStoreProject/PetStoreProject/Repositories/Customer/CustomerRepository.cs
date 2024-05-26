using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly PetStoreDBContext _context;

        public CustomerRepository(PetStoreDBContext dbContext) {
            _context = dbContext;
        }

        public int getCustomerId(string email)
        {
            var customerId = (from c in _context.Customers
                              where c.Email == email
                              select c.CustomerId).FirstOrDefault();
            return customerId;
        }
    }
}
