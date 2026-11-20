using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManageMentSystem.Services;

public class ProjectService(EmployeeManageMentSystemDbContext context, ProjectValidationService validator) : IProjectService
{
    public Task<List<Project>> GetAllAsync() => context.Projects
        .Include(project => project.Employees)
        .OrderBy(project => project.Name)
        .ToListAsync();

    public Task<Project?> GetByIdAsync(int id) => context.Projects
        .Include(project => project.Employees)
        .FirstOrDefaultAsync(project => project.Id == id);

    public async Task<string?> AddAsync(Project project, List<int> selectedEmployeeIds)
    {
        var error = validator.Validate(project);
        if (error != null) return error;

        if (await context.Projects.AnyAsync(existing => existing.Name == project.Name))
            return "A project with this name already exists.";

        project.Employees = await context.Employees
            .Where(employee => selectedEmployeeIds.Contains(employee.Id))
            .ToListAsync();

        context.Projects.Add(project);
        await context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(int id, Project project, List<int> selectedEmployeeIds)
    {
        var existing = await context.Projects
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null) return "Project was not found.";

        var error = validator.Validate(project);
        if (error != null) return error;

        if (await context.Projects.AnyAsync(other => other.Id != id && other.Name == project.Name))
            return "A project with this name already exists.";

        existing.Name = project.Name;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        existing.Status = project.Status;

        var employees = await context.Employees
            .Where(employee => selectedEmployeeIds.Contains(employee.Id))
            .ToListAsync();

        existing.Employees.Clear();
        foreach (var employee in employees)
        {
            existing.Employees.Add(employee);
        }

        await context.SaveChangesAsync();
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null) return false;

        context.Projects.Remove(project);
        await context.SaveChangesAsync();
        return true;
    }
}
