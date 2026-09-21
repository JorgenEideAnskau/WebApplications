using System.ComponentModel.DataAnnotations;

namespace Subapp1.Mvc.ViewModels;

public class SubmissionCreateViewModel
{
    public int ChallengeId { get; set; }
    public string ChallengeTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Answer { get; set; } = string.Empty;
}
