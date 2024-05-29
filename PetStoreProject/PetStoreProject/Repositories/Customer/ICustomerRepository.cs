using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Customers
{
    public interface ICustomerRepository
    {
        public int getCustomerId(string email);

        public Customer? getCustomer(string email);

        public void UpdateProfile(CustomerViewModel customer);
    }
}
