using EmployeeManageMentSystem.Models;

namespace EmployeeManageMentSystem.Services;

public class EmployeeValidationService
{
    public string? Validate(Employee employee)
    {
        if (string.IsNullOrWhiteSpace(employee.FullName)) return "Employee name is required.";
        if (employee.DepartmentId <= 0) return "Select an existing department.";
        if (employee.Salary <= 0) return "Salary must be greater than zero.";
        return null;
    }
}