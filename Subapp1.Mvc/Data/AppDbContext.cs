using Microsoft.EntityFrameworkCore;
using Subapp1.Mvc.Entities;

namespace Subapp1.Mvc.Data;

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

        modelBuilder.Entity<ChallengeTag>()
            .HasOne(ct => ct.Challenge)
            .WithMany(c => c.ChallengeTags)
            .HasForeignKey(ct => ct.ChallengeId);

        modelBuilder.Entity<ChallengeTag>()
            .HasOne(ct => ct.Tag)
            .WithMany(t => t.ChallengeTags)
            .HasForeignKey(ct => ct.TagId);

        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Code = "ITPE3200", Title = "Web Applications", Description = "Course used for practice challenges." },
            new Course { Id = 2, Code = "ITPE3300", Title = "Databases", Description = "Data modeling and SQL exercises." }
        );

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "MVC" },
            new Tag { Id = 2, Name = "API" },
            new Tag { Id = 3, Name = "Testing" }
        );

        modelBuilder.Entity<Challenge>().HasData(
            new Challenge
            {
                Id = 1,
                Title = "Explain MVC",
                Prompt = "What does MVC stand for in ASP.NET?",
                CorrectAnswer = "Model View Controller",
                MaxPoints = 10,
                MaxAttempts = 3,
                IsPublished = true,
                CourseId = 1
            }
        );

        modelBuilder.Entity<ChallengeTag>().HasData(
            new ChallengeTag { ChallengeId = 1, TagId = 1 }
        );
    }
}
