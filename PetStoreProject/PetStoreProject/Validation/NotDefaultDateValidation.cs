using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.Validation
{
    public class NotDefaultDateValidation:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateOnly date)
            {
                return date != default(DateOnly);
            }

            // Giá trị không hợp lệ nếu không phải kiểu DateOnly
            return false;
        }
    }
}
