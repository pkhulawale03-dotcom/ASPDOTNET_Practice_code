using CPositiveAPI.Model;

namespace CPositiveAPI.Interfaces
{
    public interface ICPatientRepository
    {
        IEnumerable<UserCategoryMaster> GetCategories();
        bool UsernameExists(string username);
        bool MobileExists(string mobileno);
        bool EmailExists(string emailId);

        int AddUser(Users user, UserCategoryLink categoryLink, IsRegistrationCompleted registration);
        Task<PersonalDetls> AddPersonalDetailsAsync(PersonalDetls details);

        IEnumerable<CountryMaster> GetCountries();
        IEnumerable<StateMaster> GetStates(int countryId);
        IEnumerable<DistrictMaster> GetDistricts(int stateId);
    }
}
