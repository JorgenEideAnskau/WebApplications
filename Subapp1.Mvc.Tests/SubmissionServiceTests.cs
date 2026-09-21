using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Subapp1.Mvc.Data;
using Subapp1.Mvc.Entities;
using Subapp1.Mvc.Services;

namespace Subapp1.Mvc.Tests;

public class SubmissionServiceTests
{
    private static async Task<AppDbContext> CreateDbContextAsync(string name)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;

        var db = new AppDbContext(options);
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    private static async Task SeedChallengeAsync(AppDbContext db, Challenge? challenge = null)
    {
        var existing = await db.Challenges.FindAsync(99);
        if (existing is not null)
        {
            db.Challenges.Remove(existing);
            await db.SaveChangesAsync();
        }

        db.Challenges.Add(challenge ?? new Challenge
        {
            Id = 99,
            Title = "Seed",
            Prompt = "Prompt",
            CorrectAnswer = "answer",
            MaxPoints = 10,
            MaxAttempts = 3,
            IsPublished = true,
            CourseId = 1
        });
        await db.SaveChangesAsync();
    }

    private static SubmissionService CreateService(AppDbContext db)
        => new(db, NullLogger<SubmissionService>.Instance);

    [Fact]
    public async Task SubmitAsync_CorrectAnswer_FirstAttempt_GivesMaxPoints()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_CorrectAnswer_FirstAttempt_GivesMaxPoints));
        await SeedChallengeAsync(db);
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "bob", "answer");

        Assert.True(result.Accepted);
        Assert.True(result.IsCorrect);
        Assert.Equal(10, result.AwardedPoints);
    }

    [Fact]
    public async Task SubmitAsync_CorrectAnswer_SecondAttempt_ReducesPoints()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_CorrectAnswer_SecondAttempt_ReducesPoints));
        await SeedChallengeAsync(db);
        db.Submissions.Add(new Submission { ChallengeId = 99, StudentName = "bob", Answer = "wrong", IsCorrect = false, AwardedPoints = 0 });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "bob", "answer");

        Assert.True(result.Accepted);
        Assert.True(result.IsCorrect);
        Assert.Equal(9, result.AwardedPoints);
    }

    [Fact]
    public async Task SubmitAsync_StoresSubmissionInDatabase()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_StoresSubmissionInDatabase));
        await SeedChallengeAsync(db);
        var service = CreateService(db);

        await service.SubmitAsync(99, "amy", "answer");

        Assert.Equal(1, await db.Submissions.CountAsync(s => s.StudentName == "amy"));
    }

    [Fact]
    public async Task SubmitAsync_AnswerComparison_IsCaseInsensitive()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_AnswerComparison_IsCaseInsensitive));
        await SeedChallengeAsync(db);
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "amy", "AnSwEr");

        Assert.True(result.IsCorrect);
    }

    [Fact]
    public async Task SubmitAsync_TrimsAnswerBeforeEvaluation()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_TrimsAnswerBeforeEvaluation));
        await SeedChallengeAsync(db);
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "amy", " answer ");

        Assert.True(result.IsCorrect);
    }

    [Fact]
    public async Task SubmitAsync_DifferentStudents_HaveIndependentAttempts()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_DifferentStudents_HaveIndependentAttempts));
        await SeedChallengeAsync(db);
        db.Submissions.AddRange(
            new Submission { ChallengeId = 99, StudentName = "anna", Answer = "wrong", IsCorrect = false },
            new Submission { ChallengeId = 99, StudentName = "anna", Answer = "wrong", IsCorrect = false },
            new Submission { ChallengeId = 99, StudentName = "anna", Answer = "wrong", IsCorrect = false }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "bob", "answer");

        Assert.True(result.Accepted);
        Assert.Equal(10, result.AwardedPoints);
    }

    [Fact]
    public async Task SubmitAsync_UnknownChallenge_IsRejected()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_UnknownChallenge_IsRejected));
        var service = CreateService(db);

        var result = await service.SubmitAsync(999, "bob", "answer");

        Assert.False(result.Accepted);
        Assert.Equal(0, result.AwardedPoints);
    }

    [Fact]
    public async Task SubmitAsync_DraftChallenge_IsRejected()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_DraftChallenge_IsRejected));
        await SeedChallengeAsync(db, new Challenge
        {
            Id = 99,
            Title = "Draft",
            Prompt = "Prompt",
            CorrectAnswer = "answer",
            MaxPoints = 10,
            MaxAttempts = 3,
            IsPublished = false,
            CourseId = 1
        });
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "bob", "answer");

        Assert.False(result.Accepted);
    }

    [Fact]
    public async Task SubmitAsync_MaxAttemptsReached_IsRejected()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_MaxAttemptsReached_IsRejected));
        await SeedChallengeAsync(db);
        db.Submissions.AddRange(
            new Submission { ChallengeId = 99, StudentName = "bob", Answer = "1", IsCorrect = false },
            new Submission { ChallengeId = 99, StudentName = "bob", Answer = "2", IsCorrect = false },
            new Submission { ChallengeId = 99, StudentName = "bob", Answer = "3", IsCorrect = false }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "bob", "answer");

        Assert.False(result.Accepted);
    }

    [Fact]
    public async Task SubmitAsync_WrongAnswer_GivesZeroPoints()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_WrongAnswer_GivesZeroPoints));
        await SeedChallengeAsync(db);
        var service = CreateService(db);

        var result = await service.SubmitAsync(99, "bob", "wrong");

        Assert.True(result.Accepted);
        Assert.False(result.IsCorrect);
        Assert.Equal(0, result.AwardedPoints);
    }

    [Fact]
    public async Task SubmitAsync_TrimmedStudentName_IsStored()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_TrimmedStudentName_IsStored));
        await SeedChallengeAsync(db);
        var service = CreateService(db);

        await service.SubmitAsync(99, " bob ", "answer");
        var entry = await db.Submissions.SingleAsync(s => s.ChallengeId == 99 && s.IsCorrect);

        Assert.Equal("bob", entry.StudentName);
    }

    [Fact]
    public async Task SubmitAsync_RejectedSubmission_DoesNotPersist()
    {
        var db = await CreateDbContextAsync(nameof(SubmitAsync_RejectedSubmission_DoesNotPersist));
        await SeedChallengeAsync(db, new Challenge
        {
            Id = 99,
            Title = "Draft",
            Prompt = "Prompt",
            CorrectAnswer = "answer",
            MaxPoints = 10,
            MaxAttempts = 3,
            IsPublished = false,
            CourseId = 1
        });
        var service = CreateService(db);

        await service.SubmitAsync(99, "bob", "answer");

        Assert.Equal(0, await db.Submissions.CountAsync(s => s.ChallengeId == 99 && s.StudentName == "bob"));
    }
}
