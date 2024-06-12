using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class OrganizationDetails
    {
        [Key]
        public int OrgDetailsId { get; set; }
        public int UserId { get; set; }
        public string OrgName { get; set; }
        public string OrgWebsite { get; set; }
        public string OrgEmail { get; set; }
        public string OrgMobileNumber { get; set;}
        public string OrgAddress { get; set; }
        public DateTime Createdon {  get; set; }
    }
}
