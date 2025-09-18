using CPositiveAPI.Data;
using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Repositories
{
    public class OtherDetailsRepository : IOtherDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public OtherDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddPatient(PatientDetails patient, string category)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.PatientDetails.Add(patient);
                _context.SaveChanges();

                string sql = category switch
                {
                    "Caregiver" => @"UPDATE IsRegistrationCompleted SET CaregiverPatientDetail = 'Y' WHERE UserId = @UserId",
                    "FamilyMember" => @"UPDATE IsRegistrationCompleted SET FamilyMemberPatientDetail = 'Y' WHERE UserId = @UserId",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(sql))
                {
                    _context.Database.ExecuteSqlRaw(sql, new SqlParameter("@UserId", patient.UserId));
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void AddOrganization(OrganizationDetails organization, AreaofServiceMaster areaofService)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.OrganizationDetails.Add(organization);
                _context.AreaofServiceMaster.Add(areaofService);
                _context.SaveChanges();

                string sql = @"UPDATE IsRegistrationCompleted 
                               SET OrganizationalDetails = 'Y' 
                               WHERE UserId = @UserId AND OrganizationalDetails != 'Y'";

                _context.Database.ExecuteSqlRaw(sql, new SqlParameter("@UserId", organization.UserId));

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void AddOccupation(OccupationalDetails occupation)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.OccupationalDetails.Add(occupation);
                _context.SaveChanges();

                string sql = occupation.Category switch
                {
                    "HealthcareProfessional" => @"UPDATE IsRegistrationCompleted 
                                                  SET HealthcareOccupationalDetails = 'Y' 
                                                  WHERE UserId = @UserId",
                    "MentalHealthProfessional" => @"UPDATE IsRegistrationCompleted 
                                                    SET MentalHealthOccupationalDetails = 'Y' 
                                                    WHERE UserId = @UserId",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(sql))
                {
                    _context.Database.ExecuteSqlRaw(sql, new SqlParameter("@UserId", occupation.UserId));
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public IEnumerable<PatientDetails> GetPatientDetails(int userId) =>
            _context.PatientDetails.Where(d => d.UserId == userId).ToList();

        public IEnumerable<OrganizationDetails> GetOrganizationDetails(int userId) =>
            _context.OrganizationDetails.Where(d => d.UserId == userId).ToList();

        public IEnumerable<AreaofServiceMaster> GetAreaOfServiceDetails(int userId) =>
            _context.AreaofServiceMaster.Where(d => d.UserId == userId).ToList();

        public IEnumerable<OccupationalDetails> GetOccupationalDetails(int userId) =>
            _context.OccupationalDetails.Where(d => d.UserId == userId).ToList();

        public void MarkRegistrationCompleted(int userId)
        {
            string sql = @"UPDATE IsRegistrationCompleted 
                           SET RegistrationCompleted = 'Y' 
                           WHERE UserId = @UserId AND RegistrationCompleted != 'Y'";

            _context.Database.ExecuteSqlRaw(sql, new SqlParameter("@UserId", userId));
        }
    }
}
