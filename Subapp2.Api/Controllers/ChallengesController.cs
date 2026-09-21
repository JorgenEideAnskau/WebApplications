using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subapp2.Api.Data;
using Subapp2.Api.Dtos;
using Subapp2.Api.Entities;
using Subapp2.Api.Repositories;

namespace Subapp2.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChallengesController : ControllerBase
{
    private readonly IChallengeRepository _repository;
    private readonly AppDbContext _dbContext;

    public ChallengesController(IChallengeRepository repository, AppDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ChallengeDto>>> Get([FromQuery] ChallengeSearchQuery query)
    {
        var challenges = await _repository.SearchAsync(query.Search, query.CourseId, query.TagId, query.PublishedOnly);
        return Ok(challenges.Select(MapChallenge).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ChallengeDto>> Create(ChallengeUpsertDto dto)
    {
        var challenge = new Challenge();
        await SaveChallengeFromDtoAsync(challenge, dto);
        _dbContext.Challenges.Add(challenge);
        await _dbContext.SaveChangesAsync();

        var created = await _repository.GetDetailedByIdAsync(challenge.Id);
        return CreatedAtAction(nameof(Get), new { id = challenge.Id }, MapChallenge(created!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ChallengeUpsertDto dto)
    {
        var challenge = await _dbContext.Challenges.Include(c => c.ChallengeTags).FirstOrDefaultAsync(c => c.Id == id);
        if (challenge is null)
        {
            return NotFound();
        }

        await SaveChallengeFromDtoAsync(challenge, dto);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }

    private async Task SaveChallengeFromDtoAsync(Challenge challenge, ChallengeUpsertDto dto)
    {
        challenge.Title = dto.Title;
        challenge.Prompt = dto.Prompt;
        challenge.CorrectAnswer = dto.CorrectAnswer;
        challenge.MaxPoints = dto.MaxPoints;
        challenge.MaxAttempts = dto.MaxAttempts;
        challenge.IsPublished = dto.IsPublished;
        challenge.CourseId = dto.CourseId;

        await _dbContext.Entry(challenge).Collection(c => c.ChallengeTags).LoadAsync();
        challenge.ChallengeTags.Clear();
        foreach (var tagId in dto.TagIds.Distinct())
        {
            challenge.ChallengeTags.Add(new ChallengeTag { TagId = tagId });
        }
    }

    private static ChallengeDto MapChallenge(Challenge challenge)
    {
        return new ChallengeDto(
            challenge.Id,
            challenge.Title,
            challenge.Prompt,
            challenge.CourseId,
            challenge.Course?.Title ?? string.Empty,
            challenge.ChallengeTags.Select(ct => ct.Tag?.Name ?? string.Empty).Where(n => !string.IsNullOrWhiteSpace(n)).ToList(),
            challenge.IsPublished,
            challenge.MaxPoints,
            challenge.MaxAttempts);
    }
}
