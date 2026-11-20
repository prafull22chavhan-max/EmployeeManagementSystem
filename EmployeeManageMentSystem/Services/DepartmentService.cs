using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManageMentSystem.Services;

public class DepartmentService(EmployeeManageMentSystemDbContext context) : IDepartmentService
{
    public Task<List<Department>> GetAllAsync() => context.Departments.Include(department => department.Employees).OrderBy(department => department.Name).ToListAsync();
    public Task<Department?> GetByIdAsync(int id) => context.Departments.FindAsync(id).AsTask();

    public async Task<string?> AddAsync(Department department)
    {
        if (string.IsNullOrWhiteSpace(department.Name)) return "Department name is required.";
        if (await context.Departments.AnyAsync(existing => existing.Name == department.Name)) return "A department with this name already exists.";
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(int id, Department department)
    {
        var existing = await context.Departments.FindAsync(id);
        if (existing == null) return "Department was not found.";
        if (string.IsNullOrWhiteSpace(department.Name)) return "Department name is required.";
        if (await context.Departments.AnyAsync(other => other.Id != id && other.Name == department.Name)) return "A department with this name already exists.";
        existing.Name = department.Name;
        existing.DepBudget = department.DepBudget;
        await context.SaveChangesAsync();
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await context.Departments.FindAsync(id);
        if (department == null || await context.Employees.AnyAsync(employee => employee.DepartmentId == id)) return false;
        context.Departments.Remove(department);
        await context.SaveChangesAsync();
        return true;
    }
}