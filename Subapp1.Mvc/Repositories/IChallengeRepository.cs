using Subapp1.Mvc.Entities;

namespace Subapp1.Mvc.Repositories;

public interface IChallengeRepository : IRepository<Challenge>
{
    Task<IReadOnlyList<Challenge>> SearchAsync(string? search, int? courseId, int? tagId, bool? onlyPublished, CancellationToken cancellationToken = default);
    Task<Challenge?> GetDetailedByIdAsync(int id, CancellationToken cancellationToken = default);
}
