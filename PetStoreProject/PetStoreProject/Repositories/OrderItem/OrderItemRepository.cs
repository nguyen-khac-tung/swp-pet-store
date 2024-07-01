
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.OrderItem
{
	public class OrderItemRepository : IOrderItemRepository
	{
		private readonly PetStoreDBContext _context;
		public OrderItemRepository(PetStoreDBContext context)
		{
			_context = context;
		}
		public void AddOrderItem(Models.OrderItem orderItem)
		{
			_context.OrderItems.Add(orderItem);
			_context.SaveChanges();
		}
	}
}
