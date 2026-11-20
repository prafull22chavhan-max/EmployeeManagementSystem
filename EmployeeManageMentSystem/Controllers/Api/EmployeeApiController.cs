using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers.Api;

[ApiController]
[Route("api/employees")]
public class EmployeeApiController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeResponse>>> Get(
        [FromQuery] string? search,
        [FromQuery] int? departmentId,
        [FromQuery] bool? isActive,
        [FromQuery] int? projectId)
    {
        var employees = await employeeService.GetAllAsync(search, departmentId, isActive, projectId);
        return Ok(employees.Select(ToResponse).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(EmployeeRequest request)
    {
        var employee = ToModel(request);
        var error = await employeeService.AddAsync(employee);
        if (error is not null) return BadRequest(new ApiError(error));

        var created = await employeeService.GetByIdAsync(employee.Id);
        if (created is null) return Problem("The employee was saved but could not be loaded.");
        return CreatedAtAction(nameof(Get), new { }, ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeRequest request)
    {
        var error = await employeeService.UpdateAsync(id, ToModel(request));
        return error is null ? NoContent() : BadRequest(new ApiError(error));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await employeeService.DeleteAsync(id) ? NoContent() : NotFound();

    internal static EmployeeResponse ToResponse(Employee employee) => new(
        employee.Id,
        employee.FullName,
        employee.Email,
        employee.Salary,
        employee.DepartmentId,
        employee.Department?.Name ?? string.Empty,
        employee.IsActive,
        employee.Projects.Select(project => project.Name).ToList());

    private static Employee ToModel(EmployeeRequest request) => new()
    {
        FullName = request.FullName,
        Email = request.Email,
        Salary = request.Salary,
        DepartmentId = request.DepartmentId,
        IsActive = request.IsActive
    };
}

public sealed record EmployeeRequest(
    [Required, StringLength(100)] string FullName,
    [Required, EmailAddress, StringLength(255)] string Email,
    [Range(typeof(decimal), "0.01", "1000000000")] decimal Salary,
    [Range(1, int.MaxValue)] int DepartmentId,
    bool IsActive);

public sealed record EmployeeResponse(
    int Id,
    string FullName,
    string Email,
    decimal Salary,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    IReadOnlyList<string> Projects);

public sealed record ApiError(string Message);
