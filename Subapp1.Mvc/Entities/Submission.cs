using System.ComponentModel.DataAnnotations;

namespace Subapp1.Mvc.Entities;

public class Submission
{
    public int Id { get; set; }

    public int ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }

    [Required]
    [StringLength(120)]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Answer { get; set; } = string.Empty;

    public int AwardedPoints { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
}
