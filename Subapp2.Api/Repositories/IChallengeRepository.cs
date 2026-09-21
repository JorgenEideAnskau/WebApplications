using Subapp2.Api.Entities;

namespace Subapp2.Api.Repositories;

public interface IChallengeRepository : IRepository<Challenge>
{
    Task<IReadOnlyList<Challenge>> SearchAsync(string? search, int? courseId, int? tagId, bool? publishedOnly, CancellationToken cancellationToken = default);
    Task<Challenge?> GetDetailedByIdAsync(int id, CancellationToken cancellationToken = default);
}
