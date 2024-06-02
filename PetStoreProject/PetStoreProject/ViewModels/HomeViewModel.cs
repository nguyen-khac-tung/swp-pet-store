namespace PetStoreProject.ViewModels
{
	public class HomeViewModel
	{
		public int NumberOfDogFoods { get; set; }
		public int NumberOfDogAccessories { get; set; }
		public int NumberOfCatFoods { get; set; }
		public int NumberOfCatAccessories { get; set; }

		public List<ProductDetailViewModel> DogFoodsDisplayed { get; set; }
		public List<ProductDetailViewModel> CatFoodsDisplayed { get; set; }
		public List<ProductDetailViewModel> DogAccessoriesDisplayed { get; set; }
		public List<ProductDetailViewModel> CatAccessoriesDisplayed { get; set; }
	}
}
