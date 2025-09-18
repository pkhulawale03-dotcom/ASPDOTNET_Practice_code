using CPositiveAPI.Model;

namespace CPositiveAPI.Interfaces
{
    public interface ITreatmentRepository
    {
        void AddTreatment(TreatmentConductedAt treatmentConductedAt,string category);
    }
}
