using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly NorthwindContext _db;

    public OrdersController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll() =>
        Ok(await _db.Orders.AsNoTracking().OrderBy(o => o.OrderId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == id);
        return order is null
            ? NotFound(new { message = "Pedido no encontrado." })
            : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create([FromBody] Order order)
    {
        order.OrderId = 0;
        _db.Orders.Add(order);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el pedido. Verifica el cliente, empleado y transportista.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Order order)
    {
        var existing = await _db.Orders.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Pedido no encontrado." });
        }

        order.OrderId = id;
        _db.Entry(existing).CurrentValues.SetValues(order);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el pedido. Verifica el cliente, empleado y transportista.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Orders.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Pedido no encontrado." });
        }

        _db.Orders.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar el pedido." });
        }

        return NoContent();
    }
}
