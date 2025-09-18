using CPositiveAPI.Model;

namespace CPositiveAPI.Interfaces
{
    public interface ICancerRepository
    {
        IEnumerable<CancerNameMaster> GetCancerNames();
        IEnumerable<CancerTypeMaster> GetCancerType();
        IEnumerable<GradeMaster> GetGrades();
        IEnumerable<StageMaster> GetStages();

        void AddCancerdetails(CancerInfo cancerInfo,string category);
    }
}
