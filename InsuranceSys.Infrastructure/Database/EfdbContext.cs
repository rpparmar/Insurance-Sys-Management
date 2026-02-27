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
        public DbSet<InsuranceTypeEntity> EFInsuranceTypes { get; set; }
        public DbSet<LeadStatusEntity> EFLeadStatus { get; set; }
        public DbSet<Mapping_Company_InsuranceType> EFMappingCompanyInsuranceType { get; set; }
        public DbSet<CustomerEntity> EFCustomers { get; set; }
        public DbSet<PolicyDetailsEntity> EFPolicyDetails { get; set; }
        public DbSet<PolicyVehicleDetailsEntity> EFVehicleDetails { get; set; }
        public DbSet<PolicyPaymentDetailsEntity> EFPolicyPayment { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<LeadEntity>().ToTable("LeadManagement");
            modelBuilder.Entity<UsersEntity>().ToTable("UsersInfo");
            modelBuilder.Entity<CompanyEntity>().ToTable("CompanyMaster");
            modelBuilder.Entity<InsuranceTypeEntity>().ToTable("InsuranceTypeMaster");
            modelBuilder.Entity<LeadStatusEntity>().ToTable("LeadStatus");
            modelBuilder.Entity<Mapping_Company_InsuranceType>().ToTable("Mapping_Company_InsuranceType");
            modelBuilder.Entity<CustomerEntity>().ToTable("CustomersInfo");
            modelBuilder.Entity<PolicyDetailsEntity>().ToTable("PolicyDetails");            
            modelBuilder.Entity<PolicyVehicleDetailsEntity>().ToTable("VehicleDetails");
            modelBuilder.Entity<PolicyPaymentDetailsEntity>().ToTable("PolicyPayment");            

        }
    }
}
