using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class UserCategoryMaster
    {
        [Key]
        public int UserCategoryId { get; set; }
        public string CategoryName { get; set;}
    }
}
