using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subapp1.Mvc.Entities;
using Subapp1.Mvc.Repositories;

namespace Subapp1.Mvc.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly IRepository<Course> _repository;

    public CoursesController(IRepository<Course> repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _repository.GetAllAsync());
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue)
        {
            return View(new Course());
        }

        var course = await _repository.GetByIdAsync(id.Value);
        return course is null ? NotFound() : View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Course model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Id == 0)
        {
            await _repository.AddAsync(model);
        }
        else
        {
            await _repository.UpdateAsync(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
