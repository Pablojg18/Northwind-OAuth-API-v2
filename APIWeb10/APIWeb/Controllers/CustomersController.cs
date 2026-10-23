using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly NorthwindContext _db;

    public CustomersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll() =>
        Ok(await _db.Customers.AsNoTracking().OrderBy(c => c.CustomerId).ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(string id)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerId == id);
        return customer is null
            ? NotFound(new { message = "Cliente no encontrado." })
            : Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create([FromBody] Customer customer)
    {
        customer.CustomerId = customer.CustomerId.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(customer.CustomerId))
        {
            return BadRequest(new { message = "El identificador del cliente es obligatorio." });
        }

        if (await _db.Customers.AnyAsync(c => c.CustomerId == customer.CustomerId))
        {
            return Conflict(new { message = "Ya existe un cliente con ese identificador." });
        }

        _db.Customers.Add(customer);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el cliente.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Customer customer)
    {
        var existing = await _db.Customers.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Cliente no encontrado." });
        }

        customer.CustomerId = existing.CustomerId;
        _db.Entry(existing).CurrentValues.SetValues(customer);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el cliente.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _db.Customers.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Cliente no encontrado." });
        }

        _db.Customers.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar el cliente porque tiene pedidos asociados." });
        }

        return NoContent();
    }
}
