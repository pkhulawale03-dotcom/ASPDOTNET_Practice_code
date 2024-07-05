using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class OccupationalDetails
    {
        [Key]
        public int OccupationalId { get; set; }
        public int UserId { get; set; }
        public string? Qualification { get; set; }
        public string? Specilization {  get; set; }
        public string? Experties { get; set; }
        public string? Experience { get; set; }
        public DateTime Createdon { get; set; }
    }
}
