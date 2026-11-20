using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers.Api;

[ApiController]
[Route("api/departments")]
public class DepartmentApiController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentResponse>>> Get()
    {
        var departments = await departmentService.GetAllAsync();
        return Ok(departments.Select(ToResponse).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> Create(DepartmentRequest request)
    {
        var department = new Department { Name = request.Name, DepBudget = request.DepBudget };
        var error = await departmentService.AddAsync(department);
        if (error is not null) return BadRequest(new ApiError(error));

        return CreatedAtAction(nameof(Get), new { }, ToResponse(department));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DepartmentRequest request)
    {
        var error = await departmentService.UpdateAsync(
            id,
            new Department { Id = id, Name = request.Name, DepBudget = request.DepBudget });
        return error is null ? NoContent() : BadRequest(new ApiError(error));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await departmentService.DeleteAsync(id)
            ? NoContent()
            : Conflict(new ApiError("Departments with employees cannot be deleted."));

    private static DepartmentResponse ToResponse(Department department) => new(
        department.Id,
        department.Name,
        department.DepBudget,
        department.Employees.Count);
}

public sealed record DepartmentRequest(
    [Required, StringLength(100)] string Name,
    [Range(typeof(decimal), "0", "1000000000000")] decimal DepBudget);
public sealed record DepartmentResponse(int Id, string Name, decimal DepBudget, int EmployeeCount);
