using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;

namespace Practica5.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protegemos todos los endpoints por defecto
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
    {
        return await _context.Categorias.ToListAsync();
    }

    // GET: api/categorias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound(new { message = "Categoría no encontrada." });
        }

        return categoria;
    }

    // POST: api/categorias
    [HttpPost]
    public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Opcional: Verificar si ya existe una categoría con el mismo nombre
        var existe = await _context.Categorias.AnyAsync(c => c.Nombre == categoria.Nombre);
        if (existe)
        {
            return BadRequest(new { message = "Ya existe una categoría con ese nombre." });
        }

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
    }

    // PUT: api/categorias/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
    {
        if (id != categoria.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(categoria).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoriaExists(id))
            {
                return NotFound(new { message = "Categoría no encontrada." });
            }
            else
            {
                throw;
            }
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique") == true)
        {
            return BadRequest(new { message = "Ya existe una categoría con ese nombre." });
        }

        return NoContent();
    }

    // DELETE: api/categorias/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound(new { message = "Categoría no encontrada." });
        }

        // Opcional: Verificar si hay productos asociados antes de eliminar
        var tieneProductos = await _context.Productos.AnyAsync(p => p.IdCategoria == id);
        if (tieneProductos)
        {
            return BadRequest(new { message = "No se puede eliminar la categoría porque tiene productos asociados." });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoriaExists(int id)
    {
        return _context.Categorias.Any(e => e.Id == id);
    }
}