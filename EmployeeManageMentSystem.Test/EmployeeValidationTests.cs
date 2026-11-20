using EmployeeManageMentSystem.Models;
using EmployeeManageMentSystem.Services;

namespace EmployeeManageMentSystem.Test;

public class EmployeeValidationTests
{
    private readonly EmployeeValidationService validator = new();

    [Test]
    public void Rejects_non_positive_salary()
    {
        var employee = new Employee
        {
            FullName = "Prafull Chavhan",
            Email = "prafull@example.com",
            DepartmentId = 1,
            Salary = 0
        };
        Assert.That(validator.Validate(employee), Is.EqualTo("Salary must be greater than zero."));
    }

    [Test]
    public void Accepts_valid_employee_details()
    {
        var employee = new Employee
        {
            FullName = "Prafull Chavhan",
            Email = "prafull@example.com",
            DepartmentId = 1,
            Salary = 50000
        };
        Assert.That(validator.Validate(employee), Is.Null);
    }
}