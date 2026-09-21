namespace Subapp1.Mvc.Services;

public interface ISubmissionService
{
    Task<SubmissionResult> SubmitAsync(int challengeId, string studentName, string answer, CancellationToken cancellationToken = default);
}
