using Microsoft.EntityFrameworkCore;
using TadarbProject.Models;

namespace TadarbProject.Data
{


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<FieldOfSpecialtyMaster> FieldOfSpecialtiesMaster { get; set; }

        public DbSet<FieldOfSpecialtyDetails> FieldOfSpecialtiesDetails { get; set; }

        public DbSet<UserAcount> UserAcounts { get; set; }

        public DbSet<OrganizationType> OrganizationTypes { get; set; }

        public DbSet<Organization> Organizations { get; set; }

    }
}
