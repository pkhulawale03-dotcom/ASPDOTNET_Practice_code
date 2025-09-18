using CPositiveAPI.Data;
using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Repositories
{
    public class TreatmentRepository : ITreatmentRepository
    {
        private readonly ApplicationDbContext _context;

        public TreatmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddTreatment(TreatmentConductedAt treatmentConducted, string category)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.TreatmentConductedAt.Add(treatmentConducted);
                _context.SaveChanges();

                string updatesql = string.Empty;

                if (category == "CPatient")
                {
                    updatesql = @"UPDATE IsRegistrationCompleted
                                  SET CpatientTreatmentConducted = 'Y'
                                  WHERE UserId = @UserId AND CpatientTreatmentConducted != 'Y'";
                }
                else if (category == "Caregiver")
                {
                    updatesql = @"UPDATE IsRegistrationCompleted
                                  SET CaregiverTreatmentConducted = 'Y'
                                  WHERE UserId = @UserId AND CaregiverTreatmentConducted != 'Y'";
                }
                else if (category == "Familymember")
                {
                    updatesql = @"UPDATE IsRegistrationCompleted
                                  SET FamilymemberTreatmentConducted = 'Y'
                                  WHERE UserId = @UserId AND FamilymemberTreatmentConducted != 'Y'";
                }

                _context.Database.ExecuteSqlRaw(updatesql, new[]
                {
                    new SqlParameter("UserId" ,treatmentConducted.UserId)
                });

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
    }
}
