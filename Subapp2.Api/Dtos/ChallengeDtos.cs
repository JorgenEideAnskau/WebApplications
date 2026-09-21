using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Dtos;

public record ChallengeDto(
    int Id,
    string Title,
    string Prompt,
    int CourseId,
    string CourseTitle,
    IReadOnlyList<string> Tags,
    bool IsPublished,
    int MaxPoints,
    int MaxAttempts);

public class ChallengeUpsertDto
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(400)]
    public string Prompt { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Range(1, 100)]
    public int MaxPoints { get; set; }

    [Range(1, 10)]
    public int MaxAttempts { get; set; }

    public bool IsPublished { get; set; }

    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    public List<int> TagIds { get; set; } = [];
}

public class ChallengeSearchQuery
{
    public string? Search { get; set; }
    public int? CourseId { get; set; }
    public int? TagId { get; set; }
    public bool? PublishedOnly { get; set; }
}
