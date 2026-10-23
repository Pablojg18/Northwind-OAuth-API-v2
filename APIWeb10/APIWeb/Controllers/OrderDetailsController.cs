using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderDetailsController : ControllerBase
{
    private readonly NorthwindContext _db;

    public OrderDetailsController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDetail>>> GetAll() =>
        Ok(await _db.OrderDetails.AsNoTracking()
            .OrderBy(od => od.OrderId)
            .ThenBy(od => od.ProductId)
            .ToListAsync());

    [HttpGet("{orderId:int}/{productId:int}")]
    public async Task<ActionResult<OrderDetail>> GetById(int orderId, int productId)
    {
        var detail = await _db.OrderDetails.AsNoTracking()
            .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId);

        return detail is null
            ? NotFound(new { message = "Detalle de pedido no encontrado." })
            : Ok(detail);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDetail>> Create([FromBody] OrderDetail detail)
    {
        if (await _db.OrderDetails.AnyAsync(od => od.OrderId == detail.OrderId && od.ProductId == detail.ProductId))
        {
            return Conflict(new { message = "Ese producto ya está registrado en el pedido." });
        }

        _db.OrderDetails.Add(detail);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el detalle. Verifica el pedido y el producto.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { orderId = detail.OrderId, productId = detail.ProductId }, detail);
    }

    [HttpPut("{orderId:int}/{productId:int}")]
    public async Task<IActionResult> Update(int orderId, int productId, [FromBody] OrderDetail detail)
    {
        var existing = await _db.OrderDetails.FindAsync(orderId, productId);

        if (existing is null)
        {
            return NotFound(new { message = "Detalle de pedido no encontrado." });
        }

        detail.OrderId = orderId;
        detail.ProductId = productId;
        _db.Entry(existing).CurrentValues.SetValues(detail);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el detalle.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{orderId:int}/{productId:int}")]
    public async Task<IActionResult> Delete(int orderId, int productId)
    {
        var existing = await _db.OrderDetails.FindAsync(orderId, productId);

        if (existing is null)
        {
            return NotFound(new { message = "Detalle de pedido no encontrado." });
        }

        _db.OrderDetails.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo eliminar el detalle.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }
}
