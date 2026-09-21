using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Entities;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
}
