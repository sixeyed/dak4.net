using Microsoft.EntityFrameworkCore;
using SignUp.Entities;
using System.Linq;

namespace SignUp.MessageHandlers.SaveProspectCore.Model
{
    public class SignUpContext : DbContext
    {
        public SignUpContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            SeedReferenceData();
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Prospect> Prospects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasKey(c => c.CountryCode);
            builder.Entity<Role>().HasKey(r => r.RoleCode);
            builder.Entity<Prospect>().HasOne<Country>(p => p.Country);
            builder.Entity<Prospect>().HasOne<Role>(p => p.Role);
        }

        private void SeedReferenceData()
        {
            var seeded = false;
            if (Roles.Count() == 0)
            {
                foreach (var role in Role.GetSeedData())
                {
                    Roles.Add(role);
                }
                seeded = true;
            } 

            if (Countries.Count() == 0)
            {
                foreach (var country in Country.GetSeedData())
                {
                    Countries.Add(country);                
                }
                seeded = true;
            }

            if (seeded)
            {
                this.SaveChanges();
            }
        }
    }
}
