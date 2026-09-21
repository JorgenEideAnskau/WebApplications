using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Dtos;

public class SubmissionRequestDto
{
    [Range(1, int.MaxValue)]
    public int ChallengeId { get; set; }

    [Required]
    [StringLength(120)]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Answer { get; set; } = string.Empty;
}

public record SubmissionResponseDto(bool Accepted, bool IsCorrect, int AwardedPoints, string Message);
