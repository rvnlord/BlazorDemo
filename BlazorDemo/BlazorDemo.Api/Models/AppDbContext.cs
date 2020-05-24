using BlazorDemo.Api.Common.Extensions;
using BlazorDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemo.Api.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
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

            base.OnModelCreating(mb);
            mb.Seed();
        }
    }
}
