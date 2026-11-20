using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services;

namespace EmployeeManageMentSystem.Test;

public class ProjectValidationTests
{
    private readonly ProjectValidationService validator = new();

    [Test]
    public void Rejects_project_with_end_date_before_start_date()
    {
        var project = new Project
        {
            Name = "Website Redesign",
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(5),
            Status = ProjectStatus.Active
        };

        Assert.That(validator.Validate(project), Is.EqualTo("End date must be on or after the start date."));
    }

    [Test]
    public void Accepts_valid_project_details()
    {
        var project = new Project
        {
            Name = "Website Redesign",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(20),
            Status = ProjectStatus.Active
        };

        Assert.That(validator.Validate(project), Is.Null);
    }
}
