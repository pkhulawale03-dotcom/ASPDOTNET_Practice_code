using CPositiveAPI.Model;

namespace CPositiveAPI.Interfaces
{
    public interface ICPatientRepository
    {
        IEnumerable<UserCategoryMaster> GetCategories();
        bool UsernameExists(string username);
        bool MobileExists(string mobile);
        bool EmailExists(string email);

        int AddUser(Users users,UserCategoryLink userCategoryLink,IsRegistrationCompleted isRegistration);
        Task<PersonalDetls> GetPersonalDetlsAsync(PersonalDetls personalDetls);
        IEnumerable<CountryMaster> GetCountries();
        IEnumerable<StateMaster> GetStates(int countryid);
        IEnumerable<DistrictMaster> GetDistricts(int stateid);
    }
}
