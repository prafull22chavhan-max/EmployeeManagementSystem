using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services;
using EmployeeManageMentSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManageMentSystem.Controllers;

public class DepartmentController(IDepartmentService departmentService) : Controller
{
    public async Task<IActionResult> Index() => View(await departmentService.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department department)
    {
        if (ModelState.IsValid)
        {
            var error = await departmentService.AddAsync(department);
            if (error == null) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error);
        }

        return View(department);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await departmentService.GetByIdAsync(id);
        return department == null ? NotFound() : View(department);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await departmentService.GetByIdAsync(id);
        return department == null ? NotFound() : View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Department department)
    {
        if (id != department.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            var error = await departmentService.UpdateAsync(id, department);
            if (error == null) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error);
        }

        return View(department);
    }

    [ActionName("Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await departmentService.DeleteAsync(id)) TempData["Error"] = "A department can only be deleted when it has no employees.";
        return RedirectToAction(nameof(Index));
    }
}