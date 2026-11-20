using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers;

public class ProjectController(IProjectService projectService, IEmployeeService employeeService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var projects = await projectService.GetAllAsync();
        return View(projects);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadEmployeesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project, List<int> selectedEmployeeIds)
    {
        if (ModelState.IsValid)
        {
            var error = await projectService.AddAsync(project, selectedEmployeeIds ?? new List<int>());
            if (error == null) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error);
        }

        await LoadEmployeesAsync();
        return View(project);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await projectService.GetByIdAsync(id);
        if (project == null) return NotFound();

        await LoadEmployeesAsync();
        ViewBag.SelectedEmployeeIds = project.Employees.Select(employee => employee.Id).ToList();
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project, List<int> selectedEmployeeIds)
    {
        if (id != project.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            var error = await projectService.UpdateAsync(id, project, selectedEmployeeIds ?? new List<int>());
            if (error == null) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error);
        }

        await LoadEmployeesAsync();
        ViewBag.SelectedEmployeeIds = selectedEmployeeIds ?? new List<int>();
        return View(project);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await projectService.GetByIdAsync(id);
        return project == null ? NotFound() : View(project);
    }

    [ActionName("Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await projectService.DeleteAsync(id)) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadEmployeesAsync() => ViewBag.Employees = await employeeService.GetAllAsync();
}