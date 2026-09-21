using System.ComponentModel.DataAnnotations;

namespace Subapp2.Api.Entities;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ChallengeTag> ChallengeTags { get; set; } = new List<ChallengeTag>();
}
