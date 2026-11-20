using System.ComponentModel.DataAnnotations;

namespace EmployeeManageMentSystem.Models;

public class Department
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "1000000000000")]
    [Display(Name = "Budget")]
    public decimal DepBudget { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}