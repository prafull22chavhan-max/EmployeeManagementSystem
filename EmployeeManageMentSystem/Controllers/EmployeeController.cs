using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers;

public class EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService, IProjectService projectService) : Controller
{
    public async Task<IActionResult> Index(string? search, int? departmentId, bool? isActive, int? projectId)
    {
        var employees = await employeeService.GetAllAsync(search, departmentId, isActive, projectId);
        await LoadFilterDataAsync();
        ViewBag.Search = search;
        ViewBag.DepartmentId = departmentId;
        ViewBag.IsActive = isActive;
        ViewBag.ProjectId = projectId;
        return View(employees);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadDepartmentsAsync();
        await LoadProjectsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            var error = await employeeService.AddAsync(employee);
            if (error == null) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error);
        }

        await LoadDepartmentsAsync();
        await LoadProjectsAsync();
        return View(employee);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await employeeService.GetByIdAsync(id);
        if (employee == null) return NotFound();
        await LoadDepartmentsAsync();
        await LoadProjectsAsync();
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            var error = await employeeService.UpdateAsync(id, employee);
            if (error == null) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error);
        }

        await LoadDepartmentsAsync();
        await LoadProjectsAsync();
        return View(employee);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await employeeService.GetByIdAsync(id);
        return employee == null ? NotFound() : View(employee);
    }

    [ActionName("Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await employeeService.DeleteAsync(id)) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartmentsAsync() => ViewBag.Departments = await departmentService.GetAllAsync();

    private async Task LoadProjectsAsync() => ViewBag.Projects = await projectService.GetAllAsync();

    private async Task LoadFilterDataAsync()
    {
        ViewBag.Departments = await departmentService.GetAllAsync();
        ViewBag.Projects = await projectService.GetAllAsync();
    }
}