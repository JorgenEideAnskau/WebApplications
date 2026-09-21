namespace Subapp1.Mvc.Entities;

public class ChallengeTag
{
    public int ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }

    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
