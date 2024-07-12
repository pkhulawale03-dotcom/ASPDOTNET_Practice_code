using System.ComponentModel.DataAnnotations;

namespace CPositiveAPI.Model
{
    public class IsRegistrationCompleted
    {
        [Key]
        public int CompletedId { get; set; }
        public int UserId {  get; set; }
        public string Personaldetails { get; set; }
        public string CancerInfo {  get; set; }
        public string TreatmentConducted { get; set;}
        public string PatientDetails { get; set;}
        public string OrganizationalDetails { get; set;}
        public string OccupationalDetails {  get; set;}
        public string RegistrationCompleted {  get; set;}
        public DateTime Createdon { get; set;}
        public string CpatientCancerInfo { get; set; }
        public string CaregiverCancerInfo { get; set; }
        public string FamilyMemberCancerInfo { get; set; }
        public string CpatientTreatmentConducted { get; set; }
        public string CaregiverTreatmentConducted { get; set; }
        public string FamilyMemberTreatmentConducted { get; set; }
        public string CaregiverPatientDetail { get; set; }
        public string FamilyMemberPatientDetail { get; set; }
        public string HealthcareOccupationalDetails { get; set; }
        public string MentalHealthOccupationalDetails { get; set; }

    }
}
