using EmployeeManageMentSystem.Models;

namespace EmployeeManageMentSystem.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<string?> AddAsync(Department department);
    Task<string?> UpdateAsync(int id, Department department);
    Task<bool> DeleteAsync(int id);
}