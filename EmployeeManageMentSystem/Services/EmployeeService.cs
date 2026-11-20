using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManageMentSystem.Services;

public class EmployeeService(EmployeeManageMentSystemDbContext context, EmployeeValidationService validator) : IEmployeeService
{
    public async Task<List<Employee>> GetAllAsync(string? search = null, int? departmentId = null, bool? isActive = null, int? projectId = null)
    {
        var query = context.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.Projects)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(employee =>
                employee.FullName.Contains(term) ||
                employee.Email.Contains(term));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(employee => employee.DepartmentId == departmentId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(employee => employee.IsActive == isActive.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(employee => employee.Projects.Any(project => project.Id == projectId.Value));
        }

        return await query.OrderBy(employee => employee.FullName).ToListAsync();
    }

    public Task<Employee?> GetByIdAsync(int id) => context.Employees
        .Include(employee => employee.Department)
        .Include(employee => employee.Projects)
        .FirstOrDefaultAsync(employee => employee.Id == id);

    public async Task<string?> AddAsync(Employee employee)
    {
        var error = validator.Validate(employee);
        if (error != null) return error;
        if (!await context.Departments.AnyAsync(department => department.Id == employee.DepartmentId)) return "Select an existing department.";
        if (await context.Employees.AnyAsync(existing => existing.Email == employee.Email)) return "An employee with this email already exists.";
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(int id, Employee employee)
    {
        var existing = await context.Employees.FindAsync(id);
        if (existing == null) return "Employee was not found.";
        var error = validator.Validate(employee);
        if (error != null) return error;
        if (!await context.Departments.AnyAsync(department => department.Id == employee.DepartmentId)) return "Select an existing department.";
        if (await context.Employees.AnyAsync(other => other.Id != id && other.Email == employee.Email)) return "An employee with this email already exists.";
        existing.FullName = employee.FullName;
        existing.Email = employee.Email;
        existing.Salary = employee.Salary;
        existing.DepartmentId = employee.DepartmentId;
        existing.IsActive = employee.IsActive;
        await context.SaveChangesAsync();
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);
        if (employee == null) return false;
        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
        return true;
    }
}