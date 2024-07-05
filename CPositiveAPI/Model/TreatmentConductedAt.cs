using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class TreatmentConductedAt
    {
        [Key]
        public int TreatConductedAtId { get; set; }
        public int UserId {  get; set; }
        public string? HospitalName { get; set; }
        public string? OncologistName { get; set; }
        public DateTime Createdon {  get; set; }
    }
}
