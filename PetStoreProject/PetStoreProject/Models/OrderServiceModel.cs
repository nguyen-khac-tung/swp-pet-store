namespace PetStoreProject.Models
{
    public class OrderServiceModel
    {
        public int UserId { get; set; }

        public string SortOrderServiceId { get; set; }

        public string SortName { get; set; }    

        public string SortDate { get; set; }

        public string SortTime { get; set; }

        public string SortServiceId { get; set; }

        public string SortPrice { get; set; }  
        
        public string SearchOrderServiceId { get; set; }

        public string SearchTime { get; set; }

        public string SearchName { get; set; }

        public DateOnly SearchDate { get; set; }

        public string Status { get; set; }
    }
}
