using CPositiveAPI.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CPositiveAPI.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<UserCategoryMaster> UserCategoryMaster { get; set; }
        public DbSet<Users> Users {  get; set; }
        public DbSet<UserCategoryLink> UserCategoryLinking {  get; set; }
        //public DbSet<PersonalDetls> PersonalDetails {  get; set; }
        //public DbSet<CountryMaster> CountryMaster { get; set; }
        //public DbSet<StateMaster> StateMaster { get; set; }
        //public DbSet<DistrictMaster> DistrictMaster { get; set; }
    }
}
