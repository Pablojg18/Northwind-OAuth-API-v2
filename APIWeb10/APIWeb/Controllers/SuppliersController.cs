using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly NorthwindContext _db;

    public SuppliersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetAll() =>
        Ok(await _db.Suppliers.AsNoTracking().OrderBy(s => s.SupplierId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Supplier>> GetById(int id)
    {
        var supplier = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.SupplierId == id);
        return supplier is null
            ? NotFound(new { message = "Proveedor no encontrado." })
            : Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult<Supplier>> Create([FromBody] Supplier supplier)
    {
        supplier.SupplierId = 0;
        _db.Suppliers.Add(supplier);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el proveedor.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, supplier);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Supplier supplier)
    {
        var existing = await _db.Suppliers.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Proveedor no encontrado." });
        }

        supplier.SupplierId = id;
        _db.Entry(existing).CurrentValues.SetValues(supplier);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el proveedor.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Suppliers.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Proveedor no encontrado." });
        }

        _db.Suppliers.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar el proveedor porque tiene productos asociados." });
        }

        return NoContent();
    }
}
