using CPositiveAPI.Data;
using CPositiveAPI.Interfaces;
using CPositiveAPI.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Repositories
{
    public class CPatientRepository : ICPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public CPatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserCategoryMaster> GetCategories() =>
            _context.UserCategoryMaster.ToList();

        public bool UsernameExists(string username) =>
            _context.Users.Any(u => u.Username == username);

        public bool MobileExists(string mobileno) =>
            _context.Users.Any(u => u.Mobileno == mobileno);

        public bool EmailExists(string emailId) =>
            _context.Users.Any(u => u.EmailId == emailId);

        public int AddUser(Users user, UserCategoryLink categoryLink, IsRegistrationCompleted registration)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                int userId = user.UserId;
                categoryLink.UserId = userId;
                registration.UserId = userId;

                _context.UserCategoryLinking.Add(categoryLink);
                _context.IsRegistrationCompleted.Add(registration);

                _context.SaveChanges();
                transaction.Commit();

                return userId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<PersonalDetls> AddPersonalDetailsAsync(PersonalDetls details)
        {
            _context.PersonalDetails.Add(details);
            await _context.SaveChangesAsync();

            string updateSql = @"
                UPDATE IsRegistrationCompleted
                SET Personaldetails = 'Y'
                WHERE UserId = @UserId AND Personaldetails != 'Y'";

            await _context.Database.ExecuteSqlRawAsync(updateSql, new[]
            {
                new SqlParameter("@UserId", details.UserId)
            });

            return details;
        }

        public IEnumerable<CountryMaster> GetCountries() =>
            _context.CountryMaster.ToList();

        public IEnumerable<StateMaster> GetStates(int countryId) =>
            _context.StateMaster
                .Where(s => s.CountryId == countryId)
                .OrderBy(s => s.statename)
                .ToList();

        public IEnumerable<DistrictMaster> GetDistricts(int stateId) =>
            _context.DistrictMaster
                .Where(d => d.stateid == stateId)
                .ToList();
    }
}
