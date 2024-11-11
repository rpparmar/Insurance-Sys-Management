using InsuranceSys.Domain;
using InsuranceSys.Infrastructure.EFEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database
{
    public class EfdbContext : DbContext
    {
        public EfdbContext(DbContextOptions<EfdbContext> options) : base(options) { }
        public DbSet<CountryMaster> Countries { get; set; }
        public DbSet<StateMaster> States { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryMaster>().ToTable("CountryMaster");
            //modelBuilder.Entity<CountryMaster>()
            //.HasKey(c => c.CountryID); // Set primary key here
            modelBuilder.Entity<StateMaster>().ToTable("StateMaster");
            modelBuilder.Entity<CountryMaster>()
                .HasMany(c => c.States)
                .WithOne(s => s.Country)
                .HasForeignKey(s => s.CountryID);

        }
    }
}
