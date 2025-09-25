using CPositiveAPI.Controllers;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Discussion → User : Restrict cascade
            modelBuilder.Entity<Discussion>()
                .HasOne(d => d.User)
                .WithMany() // if Users class doesn't have ICollection<Discussion>
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Discussion → Topic : Cascade delete
            modelBuilder.Entity<Discussion>()
                .HasOne(d => d.Topic)
                .WithMany(t => t.Discussions)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        }




        public DbSet<UserCategoryMaster> UserCategoryMaster { get; set; }
        public DbSet<Users> Users {  get; set; }
        public DbSet<UserCategoryLink> UserCategoryLinking {  get; set; }
        public DbSet<PersonalDetls> PersonalDetails { get; set; }
        public DbSet<CountryMaster> CountryMaster { get; set; }
        public DbSet<StateMaster> StateMaster { get; set; }
        public DbSet<DistrictMaster> DistrictMaster { get; set; }
        public DbSet<CancerInfo> CancerInfo { get; set; }
        public DbSet<CancerNameMaster>CancerNameMaster { get; set; }
        public DbSet<CancerTypeMaster> CancerTypesMaster { get; set;}
        public DbSet<GradeMaster> GradeMaster {  get; set; }
        public DbSet<StageMaster> StageMaster { get; set; }
        public DbSet<TreatmentConductedAt> TreatmentConductedAt {  get; set; }
        public DbSet<PatientDetails>PatientDetails { get; set; }
        public DbSet<OrganizationDetails> OrganizationDetails { get; set; }
        public DbSet<AreaofServiceMaster> AreaofServiceMaster { get; set;}
        public DbSet<OccupationalDetails> OccupationalDetails { get;set; }
        public DbSet<IsRegistrationCompleted> IsRegistrationCompleted {  get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Topic> Topics { get; set; }

      

    }
}
