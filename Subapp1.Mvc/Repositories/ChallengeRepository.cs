using Microsoft.EntityFrameworkCore;
using Subapp1.Mvc.Data;
using Subapp1.Mvc.Entities;

namespace Subapp1.Mvc.Repositories;

public class ChallengeRepository : Repository<Challenge>, IChallengeRepository
{
    public ChallengeRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Challenge>> SearchAsync(string? search, int? courseId, int? tagId, bool? onlyPublished, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Challenges
            .Include(c => c.Course)
            .Include(c => c.ChallengeTags)
            .ThenInclude(ct => ct.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(normalized) || c.Prompt.ToLower().Contains(normalized));
        }

        if (courseId.HasValue)
        {
            query = query.Where(c => c.CourseId == courseId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(c => c.ChallengeTags.Any(ct => ct.TagId == tagId.Value));
        }

        if (onlyPublished == true)
        {
            query = query.Where(c => c.IsPublished);
        }

        return await query
            .OrderByDescending(c => c.IsPublished)
            .ThenBy(c => c.Title)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Challenge?> GetDetailedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Challenges
            .Include(c => c.Course)
            .Include(c => c.ChallengeTags)
            .ThenInclude(ct => ct.Tag)
            .Include(c => c.Submissions)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
