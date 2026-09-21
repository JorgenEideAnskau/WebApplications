using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subapp2.Api.Dtos;
using Subapp2.Api.Services;

namespace Subapp2.Api.Controllers;

[ApiController]
[IgnoreAntiforgeryToken]
[Authorize]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionResponseDto>> Submit(SubmissionRequestDto dto)
    {
        var result = await _submissionService.SubmitAsync(dto);
        return result.Accepted ? Ok(result) : BadRequest(result);
    }
}
