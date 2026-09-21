using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subapp1.Mvc.Entities;
using Subapp1.Mvc.Repositories;
using Subapp1.Mvc.Services;
using Subapp1.Mvc.ViewModels;

namespace Subapp1.Mvc.Controllers;

[Authorize]
public class SubmissionsController : Controller
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IRepository<Submission> _submissionRepository;
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(
        IChallengeRepository challengeRepository,
        IRepository<Submission> submissionRepository,
        ISubmissionService submissionService)
    {
        _challengeRepository = challengeRepository;
        _submissionRepository = submissionRepository;
        _submissionService = submissionService;
    }

    public async Task<IActionResult> Create(int challengeId)
    {
        var challenge = await _challengeRepository.GetByIdAsync(challengeId);
        if (challenge is null)
        {
            return NotFound();
        }

        return View(new SubmissionCreateViewModel
        {
            ChallengeId = challengeId,
            ChallengeTitle = challenge.Title
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubmissionCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var result = await _submissionService.SubmitAsync(viewModel.ChallengeId, viewModel.StudentName, viewModel.Answer);
        TempData["SubmissionMessage"] = result.Message;
        TempData["SubmissionType"] = result.IsCorrect ? "success" : "warning";

        return RedirectToAction("Index", "Challenges");
    }

    public async Task<IActionResult> History()
    {
        var all = await _submissionRepository.GetAllAsync();
        var ordered = all.OrderByDescending(s => s.SubmittedAtUtc).ToList();
        return View(ordered);
    }
}
