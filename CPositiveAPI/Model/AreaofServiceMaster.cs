using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class AreaofServiceMaster
    {
        [Key]
        public int AeraofServiceId { get; set; }
        public int UserId { get; set; }
        public string IsFinancialSupport { get; set; }
        public string IsMedicalSupport {  get; set; }
        public string IsLogisticSupport { get; set; }
        public string IsCareGiverServices { get; set; }
        public string IsMentalHealthSupport { get;set;}
        public string IsTraining { get; set; }
        public string IsAwareness { get; set;}
        public string IsScreening { get; set;}
        public string IsOther {  get; set; }
        public string IfOtherTestHere { get; set; }
        public DateTime Createdon { get; set; }
    }
}
