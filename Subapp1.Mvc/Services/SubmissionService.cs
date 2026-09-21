using Microsoft.EntityFrameworkCore;
using Subapp1.Mvc.Data;
using Subapp1.Mvc.Entities;

namespace Subapp1.Mvc.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext dbContext, ILogger<SubmissionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SubmissionResult> SubmitAsync(int challengeId, string studentName, string answer, CancellationToken cancellationToken = default)
    {
        var challenge = await _dbContext.Challenges.FirstOrDefaultAsync(c => c.Id == challengeId, cancellationToken);
        if (challenge is null || !challenge.IsPublished)
        {
            _logger.LogWarning("Rejected submission for challenge {ChallengeId}: challenge not available", challengeId);
            return new SubmissionResult(false, false, 0, "Challenge is not available.");
        }

        var attempts = await _dbContext.Submissions.CountAsync(
            s => s.ChallengeId == challengeId && s.StudentName == studentName,
            cancellationToken);

        if (attempts >= challenge.MaxAttempts)
        {
            _logger.LogInformation("Rejected submission for challenge {ChallengeId}: max attempts reached by {StudentName}", challengeId, studentName);
            return new SubmissionResult(false, false, 0, "Maximum attempts reached for this challenge.");
        }

        var normalizedAnswer = answer.Trim().ToLowerInvariant();
        var normalizedCorrect = challenge.CorrectAnswer.Trim().ToLowerInvariant();
        var isCorrect = normalizedAnswer == normalizedCorrect;
        var awardedPoints = isCorrect ? challenge.MaxPoints - attempts : 0;

        var submission = new Submission
        {
            ChallengeId = challengeId,
            StudentName = studentName.Trim(),
            Answer = answer.Trim(),
            IsCorrect = isCorrect,
            AwardedPoints = Math.Max(awardedPoints, 0),
            SubmittedAtUtc = DateTime.UtcNow
        };

        _dbContext.Submissions.Add(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Submission stored for challenge {ChallengeId} by {StudentName} with {Points} points", challengeId, studentName, submission.AwardedPoints);

        return isCorrect
            ? new SubmissionResult(true, true, submission.AwardedPoints, "Correct answer. Great work!")
            : new SubmissionResult(true, false, 0, "Incorrect answer. Try again.");
    }
}
