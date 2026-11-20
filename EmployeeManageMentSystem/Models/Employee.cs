using System.ComponentModel.DataAnnotations;

namespace EmployeeManageMentSystem.Models;



public class Employee
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000000")]
    public decimal Salary { get; set; }

    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public Department? Department { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}