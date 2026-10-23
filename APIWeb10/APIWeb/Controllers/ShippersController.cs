using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShippersController : ControllerBase
{
    private readonly NorthwindContext _db;

    public ShippersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Shipper>>> GetAll() =>
        Ok(await _db.Shippers.AsNoTracking().OrderBy(s => s.ShipperId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Shipper>> GetById(int id)
    {
        var shipper = await _db.Shippers.AsNoTracking().FirstOrDefaultAsync(s => s.ShipperId == id);
        return shipper is null
            ? NotFound(new { message = "Transportista no encontrado." })
            : Ok(shipper);
    }

    [HttpPost]
    public async Task<ActionResult<Shipper>> Create([FromBody] Shipper shipper)
    {
        shipper.ShipperId = 0;
        _db.Shippers.Add(shipper);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el transportista.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = shipper.ShipperId }, shipper);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Shipper shipper)
    {
        var existing = await _db.Shippers.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Transportista no encontrado." });
        }

        shipper.ShipperId = id;
        _db.Entry(existing).CurrentValues.SetValues(shipper);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el transportista.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Shippers.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Transportista no encontrado." });
        }

        _db.Shippers.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar el transportista porque tiene pedidos asociados." });
        }

        return NoContent();
    }
}
