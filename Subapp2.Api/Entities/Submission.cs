namespace Subapp2.Api.Entities;

public class Submission
{
    public int Id { get; set; }
    public int ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int AwardedPoints { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
}
