using EmployeeManageMentSystem.Models;

namespace EmployeeManageMentSystem.Services;

public class ProjectValidationService
{
    public string? Validate(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Name)) return "Project name is required.";
        if (project.StartDate == default) return "Start date is required.";
        if (project.EndDate == default) return "End date is required.";
        if (project.EndDate < project.StartDate) return "End date must be on or after the start date.";
        return null;
    }
}
