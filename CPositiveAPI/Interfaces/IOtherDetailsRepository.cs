using CPositiveAPI.Model;

namespace CPositiveAPI.Interfaces
{
    public interface IOtherDetailsRepository
    {
        void AddPatient(PatientDetails patient, string category);
        void AddOrganization(OrganizationDetails organization, AreaofServiceMaster areaofService);
        void AddOccupation(OccupationalDetails occupation);

        IEnumerable<PatientDetails> GetPatientDetails(int userId);
        IEnumerable<OrganizationDetails> GetOrganizationDetails(int userId);
        IEnumerable<AreaofServiceMaster> GetAreaOfServiceDetails(int userId);
        IEnumerable<OccupationalDetails> GetOccupationalDetails(int userId);

        void MarkRegistrationCompleted(int userId);
    }
}
