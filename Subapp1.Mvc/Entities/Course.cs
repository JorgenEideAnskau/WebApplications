using System.ComponentModel.DataAnnotations;

namespace Subapp1.Mvc.Entities;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
}
