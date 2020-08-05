using System;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.EmployeeManagement;
using BlazorDemo.Common.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Common.Models
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CryptographyKey> CryptographyKeys { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.RenameIdentityTables();

            mb.Entity<Department>().ToTable("Departments").HasKey(d => d.Id);
            mb.Entity<Department>().Property(d => d.Id).ValueGeneratedNever();

            mb.Entity<Employee>().ToTable("Employees").HasKey(e => e.Id);
            mb.Entity<Employee>().Property(e => e.Id).ValueGeneratedOnAdd();
            mb.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            mb.Entity<CryptographyKey>().ToTable("CryptographyKeys").HasKey(e => e.Name);

            mb.Seed();
        }

        public static AppDbContext Create()
        {
            var o = new DbContextOptionsBuilder<AppDbContext>();
            o.UseSqlServer(ConfigUtils.DBCS);
            return new AppDbContext(o.Options);
        }
    }
}
