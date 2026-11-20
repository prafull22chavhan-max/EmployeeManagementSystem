using EmployeeManageMentSystem.Models;

namespace EmployeeManageMentSystem.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync(string? search = null, int? departmentId = null, bool? isActive = null, int? projectId = null);
    Task<Employee?> GetByIdAsync(int id);
    Task<string?> AddAsync(Employee employee);
    Task<string?> UpdateAsync(int id, Employee employee);
    Task<bool> DeleteAsync(int id);
}