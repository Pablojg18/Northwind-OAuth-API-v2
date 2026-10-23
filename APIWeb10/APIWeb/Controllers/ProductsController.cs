using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly NorthwindContext _db;

    public ProductsController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll() =>
        Ok(await _db.Products.AsNoTracking().OrderBy(p => p.ProductId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
        return product is null
            ? NotFound(new { message = "Producto no encontrado." })
            : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        product.ProductId = 0;
        _db.Products.Add(product);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el producto. Verifica la categoría y el proveedor.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        var existing = await _db.Products.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        product.ProductId = id;
        _db.Entry(existing).CurrentValues.SetValues(product);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el producto. Verifica la categoría y el proveedor.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Products.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        _db.Products.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar el producto porque tiene pedidos asociados." });
        }

        return NoContent();
    }
}
