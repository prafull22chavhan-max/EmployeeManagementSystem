using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeManageMentSystem.Models;

namespace EmployeeManageMentSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        var message = statusCode switch
        {
            400 => "The request could not be processed.",
            401 => "You are not authorized to perform this action.",
            403 => "Access to this resource is forbidden.",
            404 => "The requested record was not found.",
            500 => "The server encountered an error while processing your request.",
            _ => "An unexpected error occurred. Please try again later."
        };

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode,
            Message = message
        });
    }
}
