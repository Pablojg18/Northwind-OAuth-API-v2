using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly NorthwindContext _db;

    public CategoriesController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll() =>
        Ok(await _db.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
        return category is null
            ? NotFound(new { message = "Categoría no encontrada." })
            : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] Category category)
    {
        category.CategoryId = 0;
        _db.Categories.Add(category);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear la categoría.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Category category)
    {
        var existing = await _db.Categories.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Categoría no encontrada." });
        }

        category.CategoryId = id;
        _db.Entry(existing).CurrentValues.SetValues(category);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar la categoría.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Categories.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Categoría no encontrada." });
        }

        _db.Categories.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar la categoría porque tiene productos asociados." });
        }

        return NoContent();
    }
}
