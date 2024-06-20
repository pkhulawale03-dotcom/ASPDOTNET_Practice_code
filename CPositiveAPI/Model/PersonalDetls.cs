using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPositiveAPI.Model
{
    public class PersonalDetls
    {
        [Key]
        public int PdId { get; set; }
        public int UserId { get; set; }//forigen key from users
        public string Name { get; set; }
        public int CountryId { get; set; }//forigen key from countrymaster
        public int StateId { get; set; }//forigen key from statemaster
        public int DistrictId { get; set; }//forigen key from districtmaster
        public string Address { get; set; }
        public string Pincode { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string HighestQualification { get; set; }
        public string Occupation { get; set; }
        public DateTime Createdon { get; set; }

    }
}
