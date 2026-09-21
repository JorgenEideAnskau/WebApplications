using Microsoft.EntityFrameworkCore;
using Subapp2.Api.Entities;

namespace Subapp2.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ChallengeTag> ChallengeTags => Set<ChallengeTag>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChallengeTag>().HasKey(ct => new { ct.ChallengeId, ct.TagId });

        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Code = "ITPE3200", Title = "Web Applications" },
            new Course { Id = 2, Code = "ITPE3400", Title = "Cloud Deployment" }
        );

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "frontend" },
            new Tag { Id = 2, Name = "backend" },
            new Tag { Id = 3, Name = "testing" }
        );

        modelBuilder.Entity<Challenge>().HasData(
            new Challenge
            {
                Id = 1,
                CourseId = 1,
                Title = "REST basics",
                Prompt = "What HTTP method is typically used for updates?",
                CorrectAnswer = "PUT",
                MaxPoints = 10,
                MaxAttempts = 3,
                IsPublished = true
            }
        );

        modelBuilder.Entity<ChallengeTag>().HasData(
            new ChallengeTag { ChallengeId = 1, TagId = 2 }
        );
    }
}
