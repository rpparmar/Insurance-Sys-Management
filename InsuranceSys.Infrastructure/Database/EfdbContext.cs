using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
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
        public DbSet<LeadEntity> EFLeads { get; set; }
        public DbSet<UsersEntity> EFUsers { get; set; }
        public DbSet<CompanyEntity> EFCompanies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<LeadEntity>().ToTable("LeadManagement");
            modelBuilder.Entity<UsersEntity>().ToTable("UsersInfo");
            modelBuilder.Entity<CompanyEntity>().ToTable("CompanyMaster");

        }
    }
}
