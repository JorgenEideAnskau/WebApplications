namespace Subapp1.Mvc.Services;

public record SubmissionResult(bool Accepted, bool IsCorrect, int AwardedPoints, string Message);
