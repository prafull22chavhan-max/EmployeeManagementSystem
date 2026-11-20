using System.ComponentModel.DataAnnotations;

namespace EmployeeManageMentSystem.Models;

public class Project
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, Display(Name = "Start Date"),]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required, Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public enum ProjectStatus
{
    Planned = 0,
    Active = 1,
    Completed = 2,
    OnHold = 3
}
