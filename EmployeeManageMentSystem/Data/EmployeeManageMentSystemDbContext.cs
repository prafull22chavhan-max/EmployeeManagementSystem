using Microsoft.EntityFrameworkCore;

namespace EmployeeManageMentSystem.Models;

public class EmployeeManageMentSystemDbContext(DbContextOptions<EmployeeManageMentSystemDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasIndex(department => department.Name).IsUnique();
        modelBuilder.Entity<Department>().Property(department => department.DepBudget).HasPrecision(18, 2);

        modelBuilder.Entity<Employee>().HasIndex(employee => employee.Email).IsUnique();
        modelBuilder.Entity<Employee>().Property(employee => employee.Salary).HasPrecision(18, 2);
        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.Department)
            .WithMany(department => department.Employees)
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasMany(employee => employee.Projects)
            .WithMany(project => project.Employees)
            .UsingEntity("EmployeeProjects");

        modelBuilder.Entity<Project>().Property(project => project.Name).HasMaxLength(200);
        modelBuilder.Entity<Project>().HasIndex(project => project.Name).IsUnique();
    }
}
