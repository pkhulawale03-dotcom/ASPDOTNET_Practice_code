using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class UserCategoryLink
    {
        [Key]
        public int UserLinkingId { get; set; }
        public int UserId { get; set; }
        public string CPositive { get; set; }
        public string Caregiver { get; set; }
        public string FamilyMember { get; set; }
        public string Volunteer { get; set; }
        public string HealthcareProfessional { get; set; }
        public string MentalHealthProfessional { get; set; }
    }
}
