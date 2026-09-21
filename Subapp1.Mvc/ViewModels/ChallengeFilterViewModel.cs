using Microsoft.AspNetCore.Mvc.Rendering;
using Subapp1.Mvc.Entities;

namespace Subapp1.Mvc.ViewModels;

public class ChallengeFilterViewModel
{
    public string? Search { get; set; }
    public int? CourseId { get; set; }
    public int? TagId { get; set; }
    public bool OnlyPublished { get; set; }

    public IReadOnlyList<Challenge> Challenges { get; set; } = Array.Empty<Challenge>();
    public List<SelectListItem> Courses { get; set; } = [];
    public List<SelectListItem> Tags { get; set; } = [];
}
