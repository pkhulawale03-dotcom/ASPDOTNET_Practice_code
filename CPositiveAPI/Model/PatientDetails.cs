using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class PatientDetails
    {
        [Key]
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string? RelWithPatient {  get; set; }
        public DateTime Createdon {  get; set; }
    }
}
