using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Entities;

public class Challenge
{
    public int Id { get; set; }

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
    public int MaxPoints { get; set; } = 10;

    [Range(1, 10)]
    public int MaxAttempts { get; set; } = 3;

    public bool IsPublished { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public ICollection<ChallengeTag> ChallengeTags { get; set; } = new List<ChallengeTag>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
