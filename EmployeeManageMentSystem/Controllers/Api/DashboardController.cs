using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(
    IEmployeeService employeeService,
    IDepartmentService departmentService,
    IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardSummary>> Get()
    {
        var employees = await employeeService.GetAllAsync();
        var departments = await departmentService.GetAllAsync();
        var projects = await projectService.GetAllAsync();

        var summary = new DashboardSummary(
            employees.Count,
            employees.Count(employee => employee.IsActive),
            departments.Count,
            projects.Count,
            projects.Count(project => project.Status == ProjectStatus.Active),
            employees.Where(employee => employee.IsActive).Sum(employee => employee.Salary),
            departments.Select(department => new DepartmentSummary(
                department.Id,
                department.Name,
                department.Employees.Count)).ToList(),
            projects.GroupBy(project => project.Status)
                .ToDictionary(group => group.Key.ToString(), group => group.Count()),
            employees.OrderByDescending(employee => employee.Id)
                .Take(5)
                .Select(EmployeeApiController.ToResponse)
                .ToList());

        return Ok(summary);
    }
}

public sealed record DashboardSummary(
    int TotalEmployees,
    int ActiveEmployees,
    int TotalDepartments,
    int TotalProjects,
    int ActiveProjects,
    decimal TotalAnnualPayroll,
    IReadOnlyList<DepartmentSummary> DepartmentDistribution,
    IReadOnlyDictionary<string, int> ProjectsByStatus,
    IReadOnlyList<EmployeeResponse> RecentEmployees);

public sealed record DepartmentSummary(int Id, string Name, int EmployeeCount);
