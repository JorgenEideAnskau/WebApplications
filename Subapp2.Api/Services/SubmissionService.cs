using Microsoft.EntityFrameworkCore;
using Subapp2.Api.Data;
using Subapp2.Api.Dtos;
using Subapp2.Api.Entities;

namespace Subapp2.Api.Services;

public static class LogSanitizer
{
    public static string Clean(string? value)
        => (value ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
}

public class SubmissionService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext dbContext, ILogger<SubmissionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SubmissionResponseDto> SubmitAsync(SubmissionRequestDto dto, CancellationToken cancellationToken = default)
    {
        var challenge = await _dbContext.Challenges.FirstOrDefaultAsync(c => c.Id == dto.ChallengeId, cancellationToken);
        if (challenge is null || !challenge.IsPublished)
        {
            return new SubmissionResponseDto(false, false, 0, "Challenge is not available.");
        }

        var attempts = await _dbContext.Submissions.CountAsync(
            s => s.ChallengeId == dto.ChallengeId && s.StudentName == dto.StudentName,
            cancellationToken);

        if (attempts >= challenge.MaxAttempts)
        {
            _logger.LogWarning("Submission rejected for student {StudentName} due to max attempts", LogSanitizer.Clean(dto.StudentName));
            return new SubmissionResponseDto(false, false, 0, "Maximum attempts reached.");
        }

        var isCorrect = dto.Answer.Trim().Equals(challenge.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
        var points = isCorrect ? Math.Max(challenge.MaxPoints - attempts, 0) : 0;

        _dbContext.Submissions.Add(new Submission
        {
            ChallengeId = dto.ChallengeId,
            StudentName = dto.StudentName.Trim(),
            Answer = dto.Answer.Trim(),
            IsCorrect = isCorrect,
            AwardedPoints = points,
            SubmittedAtUtc = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return isCorrect
            ? new SubmissionResponseDto(true, true, points, "Correct answer")
            : new SubmissionResponseDto(true, false, 0, "Incorrect answer");
    }
}
