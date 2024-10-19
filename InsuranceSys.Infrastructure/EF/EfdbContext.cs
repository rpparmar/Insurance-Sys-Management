using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.EF
{
    public class EfdbContext : DbContext
    {
        public EfdbContext(DbContextOptions<EfdbContext> options) : base(options) { }
        //public DbSet<Country> Countries { get; set; }
        //public DbSet<State> States { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Country>().ToTable("Countries");
            //modelBuilder.Entity<State>().ToTable("States");
            //modelBuilder.Entity<Country>()
            //    .HasMany(c => c.States)
            //    .WithOne(s => s.Country)
            //    .HasForeignKey(s => s.CountryId);
        }
    }
}
