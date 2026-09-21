using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subapp2.Api.Dtos;
using Subapp2.Api.Entities;
using Subapp2.Api.Repositories;

namespace Subapp2.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IRepository<Tag> _repository;

    public TagsController(IRepository<Tag> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items.Select(t => new TagDto(t.Id, t.Name)).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(TagUpsertDto dto)
    {
        var entity = await _repository.AddAsync(new Tag { Name = dto.Name });
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, new TagDto(entity.Id, entity.Name));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TagUpsertDto dto)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        item.Name = dto.Name;
        await _repository.UpdateAsync(item);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
