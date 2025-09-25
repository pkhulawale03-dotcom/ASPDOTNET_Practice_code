using CPositiveAPI.Data;
using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Repositories
{
    public class CancerDetailsRepository :ICancerRepository
    {
        private readonly ApplicationDbContext _context;

        public CancerDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CancerNameMaster> GetCancerNames() => 
            _context.CancerNameMaster.ToList();

        public IEnumerable<CancerTypeMaster> GetCancerType() =>
            _context.CancerTypesMaster.ToList();

        public IEnumerable<GradeMaster> GetGrades() =>
            _context.GradeMaster.ToList();

        public IEnumerable<StageMaster> GetStages() =>
            _context.StageMaster.ToList();

        public void AddCancerdetails(CancerInfo cancerInfo, string category)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.CancerInfo.Add(cancerInfo);
                _context.SaveChanges();

                string updatesql = null;

                if (category == "Cpatient")
                {
                    updatesql = @"UPDATE IsRegistrationCompleted
                          SET CpatientCancerInfo = 'Y'
                          WHERE UserId = @UserId";
                }
                else if (category == "Caregiver")
                {
                    updatesql = @"UPDATE IsRegistrationCompleted
                          SET CaregiverCancerInfo = 'Y'
                          WHERE UserId = @UserId";
                }
                else if (category == "Familymember")
                {
                    updatesql = @"UPDATE IsRegistrationCompleted
                          SET FamilymemberCancerInfo = 'Y'
                          WHERE UserId = @UserId";
                }

                if (!string.IsNullOrEmpty(updatesql))
                {
                    _context.Database.ExecuteSqlRaw(updatesql, new[]
                    {
                new SqlParameter("@UserId", cancerInfo.UserId) // added @ for safety
            });
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
