using EmployeeManageMentSystem.Models;

namespace EmployeeManageMentSystem.Services.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<string?> AddAsync(Project project, List<int> selectedEmployeeIds);
    Task<string?> UpdateAsync(int id, Project project, List<int> selectedEmployeeIds);
    Task<bool> DeleteAsync(int id);
}
