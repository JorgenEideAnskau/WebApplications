using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subapp2.Api.Dtos;
using Subapp2.Api.Entities;
using Subapp2.Api.Repositories;

namespace Subapp2.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IRepository<Course> _repository;

    public CoursesController(IRepository<Course> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items.Select(c => new CourseDto(c.Id, c.Code, c.Title)).ToList());
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(CourseUpsertDto dto)
    {
        var entity = await _repository.AddAsync(new Course { Code = dto.Code, Title = dto.Title });
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, new CourseDto(entity.Id, entity.Code, entity.Title));
    }

    [ValidateAntiForgeryToken]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CourseUpsertDto dto)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        item.Code = dto.Code;
        item.Title = dto.Title;
        await _repository.UpdateAsync(item);
        return NoContent();
    }

    [ValidateAntiForgeryToken]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
