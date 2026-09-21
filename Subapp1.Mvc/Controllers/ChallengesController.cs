using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Subapp1.Mvc.Data;
using Subapp1.Mvc.Entities;
using Subapp1.Mvc.Repositories;
using Subapp1.Mvc.ViewModels;
using Subapp1.Mvc.Services;

namespace Subapp1.Mvc.Controllers;

[Authorize]
public class ChallengesController : Controller
{
    private readonly IChallengeRepository _repository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ChallengesController> _logger;

    public ChallengesController(
        IChallengeRepository repository,
        IRepository<Course> courseRepository,
        IRepository<Tag> tagRepository,
        AppDbContext dbContext,
        ILogger<ChallengesController> logger)
    {
        _repository = repository;
        _courseRepository = courseRepository;
        _tagRepository = tagRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IActionResult> Index([FromQuery] ChallengeFilterViewModel filter)
    {
        filter.Challenges = await _repository.SearchAsync(filter.Search, filter.CourseId, filter.TagId, filter.OnlyPublished);
        filter.Courses = (await _courseRepository.GetAllAsync()).Select(c => new SelectListItem(c.Title, c.Id.ToString())).ToList();
        filter.Tags = (await _tagRepository.GetAllAsync()).Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
        return View(filter);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var model = new ChallengeEditViewModel();

        if (id.HasValue)
        {
            var challenge = await _repository.GetDetailedByIdAsync(id.Value);
            if (challenge is null)
            {
                return NotFound();
            }

            model = new ChallengeEditViewModel
            {
                Id = challenge.Id,
                Title = challenge.Title,
                Prompt = challenge.Prompt,
                CorrectAnswer = challenge.CorrectAnswer,
                MaxPoints = challenge.MaxPoints,
                MaxAttempts = challenge.MaxAttempts,
                IsPublished = challenge.IsPublished,
                CourseId = challenge.CourseId,
                SelectedTagIds = challenge.ChallengeTags.Select(ct => ct.TagId).ToList()
            };
        }

        await PopulateSelectDataAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ChallengeEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectDataAsync();
            return View(model);
        }

        Challenge challenge;
        if (model.Id == 0)
        {
            challenge = new Challenge();
            _dbContext.Challenges.Add(challenge);
        }
        else
        {
            challenge = await _dbContext.Challenges.Include(c => c.ChallengeTags).FirstOrDefaultAsync(c => c.Id == model.Id)
                ?? throw new InvalidOperationException("Challenge no longer exists");
        }

        challenge.Title = model.Title;
        challenge.Prompt = model.Prompt;
        challenge.CorrectAnswer = model.CorrectAnswer;
        challenge.MaxPoints = model.MaxPoints;
        challenge.MaxAttempts = model.MaxAttempts;
        challenge.IsPublished = model.IsPublished;
        challenge.CourseId = model.CourseId;

        challenge.ChallengeTags.Clear();
        foreach (var tagId in model.SelectedTagIds.Distinct())
        {
            challenge.ChallengeTags.Add(new ChallengeTag { Challenge = challenge, TagId = tagId });
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Challenge {ChallengeTitle} saved", LogSanitizer.Clean(challenge.Title));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSelectDataAsync()
    {
        ViewBag.Courses = (await _courseRepository.GetAllAsync()).Select(c => new SelectListItem(c.Title, c.Id.ToString())).ToList();
        ViewBag.Tags = (await _tagRepository.GetAllAsync()).Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
    }
}
