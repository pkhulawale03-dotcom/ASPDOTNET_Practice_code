using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class CountryMaster
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryName { get; set; }

    }
}
