using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SignUp.Entities;

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
            if (this.Roles.Count() == 0)
            {
                AddRole("-", "--Not telling");
                AddRole("DA", "Developer Advocate");
                AddRole("DM", "Decision Maker");
                AddRole("AC", "Architect");
                AddRole("EN", "Engineer");
                AddRole("OP", "IT Ops");
                seeded = true;
            }

            if (this.Countries.Count() == 0)
            {
                AddCountry("-", "--Not telling");
                AddCountry("GBR", "United Kingdom");
                AddCountry("USA", "United States");
                AddCountry("PT", "Portugal");
                AddCountry("NOR", "Norway");
                AddCountry("SWE", "Sweden");
                AddCountry("IRE", "Ireland");
                AddCountry("DMK", "Denmark");            
                AddCountry("IND", "India");
                AddCountry("LIT", "Lithuania");
                AddCountry("SPN", "Spain");
                AddCountry("BGM", "Belgium");
                AddCountry("TNL", "The Netherlands");            
                AddCountry("POL", "Poland");
                seeded = true;
            }

            if (seeded)
            {
                this.SaveChanges();
            }
        }

        private void AddCountry(string code, string name)
        {
            this.Countries.Add(new Country
            {
                CountryCode = code,
                CountryName = name
            });
        }

        private void AddRole(string code, string name)
        {
            this.Roles.Add(new Role
            {
                RoleCode = code,
                RoleName = name
            });
        }
    }
}
