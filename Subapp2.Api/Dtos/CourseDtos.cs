using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Dtos;

public record CourseDto(int Id, string Code, string Title);

public class CourseUpsertDto
{
    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;
}
