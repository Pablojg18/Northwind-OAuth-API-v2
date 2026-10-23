using APIWeb.Data;
using APIWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly NorthwindContext _db;

    public EmployeesController(NorthwindContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll() =>
        Ok(await _db.Employees.AsNoTracking().OrderBy(e => e.EmployeeId).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetById(int id)
    {
        var employee = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == id);
        return employee is null
            ? NotFound(new { message = "Empleado no encontrado." })
            : Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
    {
        employee.EmployeeId = 0;
        _db.Employees.Add(employee);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo crear el empleado. Verifica el jefe asignado.", detail = ex.InnerException?.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, employee);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
    {
        var existing = await _db.Employees.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Empleado no encontrado." });
        }

        employee.EmployeeId = id;

        if (employee.ReportsTo == id)
        {
            employee.ReportsTo = null;
        }

        _db.Entry(existing).CurrentValues.SetValues(employee);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { message = "No se pudo actualizar el empleado. Verifica el jefe asignado.", detail = ex.InnerException?.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Employees.FindAsync(id);

        if (existing is null)
        {
            return NotFound(new { message = "Empleado no encontrado." });
        }

        _db.Employees.Remove(existing);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "No se puede eliminar el empleado porque tiene pedidos asociados." });
        }

        return NoContent();
    }
}
