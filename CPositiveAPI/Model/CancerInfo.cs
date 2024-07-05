using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class CancerInfo
    {
        [Key]
        public int Cancerinfoid { get; set; }
        public int UserId { get; set; }
        public int CancertypeId { get; set; }
        public int CancerNameId { get; set; }
        public int StageId { get; set; }
        public int GradeId { get; set; }
        public string? IsFirstTime { get; set; }
        public string? IsRelapsed { get; set; }
        public string? IsTreatmentOngoing { get; set; }
        public string? IsSurgery { get; set; }
        public string? IsChemo { get; set; }
        public string? IsRadiation { get; set; }
        public string? IsTargetedTherapy { get; set; }
        public string? IsPallitiveCare { get; set; }
        public string? IsRemission { get; set; }
        public DateTime Createdon { get; set; }
    }
}
