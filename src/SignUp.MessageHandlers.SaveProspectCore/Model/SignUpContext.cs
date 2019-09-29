using System;
using Microsoft.EntityFrameworkCore;
using SignUp.Entities;

namespace SignUp.MessageHandlers.SaveProspectCore.Model
{
    public class SignUpContext : DbContext
    {
        public SignUpContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
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
    }
}
