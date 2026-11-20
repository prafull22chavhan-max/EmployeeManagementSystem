using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers.Api;

[ApiController]
[Route("api/projects")]
public class ProjectApiController(
    IProjectService projectService,
    IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> Get()
    {
        var projects = await projectService.GetAllAsync();
        return Ok(projects.Select(ToResponse).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(ProjectRequest request)
    {
        var project = ToModel(request);
        var error = await projectService.AddAsync(project, request.EmployeeIds);
        if (error is not null) return BadRequest(new ApiError(error));

        var created = await projectService.GetByIdAsync(project.Id);
        if (created is null) return Problem("The project was saved but could not be loaded.");
        return CreatedAtAction(nameof(Get), new { }, ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectRequest request)
    {
        var error = await projectService.UpdateAsync(id, ToModel(request), request.EmployeeIds);
        return error is null ? NoContent() : BadRequest(new ApiError(error));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await projectService.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpGet("~/api/project-options")]
    public async Task<ActionResult<IReadOnlyList<EmployeeOption>>> GetEmployeeOptions()
    {
        var employees = await employeeService.GetAllAsync();
        return Ok(employees.Select(employee => new EmployeeOption(employee.Id, employee.FullName)).ToList());
    }

    private static Project ToModel(ProjectRequest request) => new()
    {
        Name = request.Name,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        Status = request.Status
    };

    private static ProjectResponse ToResponse(Project project) => new(
        project.Id,
        project.Name,
        project.StartDate,
        project.EndDate,
        project.Status.ToString(),
        project.Employees.Count,
        project.Employees.Select(employee => employee.Id).ToList());
}

public sealed record ProjectRequest(
    [Required, StringLength(200)] string Name,
    [Required] DateTime StartDate,
    [Required] DateTime EndDate,
    [EnumDataType(typeof(ProjectStatus))]
    ProjectStatus Status,
    [Required]
    List<int> EmployeeIds);

public sealed record ProjectResponse(
    int Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    int EmployeeCount,
    IReadOnlyList<int> EmployeeIds);

public sealed record EmployeeOption(int Id, string FullName);
