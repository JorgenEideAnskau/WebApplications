using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Dtos;

public record TagDto(int Id, string Name);

public class TagUpsertDto
{
    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;
}
