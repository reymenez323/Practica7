using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;

namespace Practica5.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // ========== ENDPOINTS CRUD BÁSICOS ==========

    // GET: api/productos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
    {
        // Incluimos las relaciones para mostrar los nombres en lugar de solo IDs
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .ToListAsync();
    }

    // GET: api/productos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProducto(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        return producto;
    }

    // POST: api/productos
    [HttpPost]
    public async Task<ActionResult<Producto>> PostProducto(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validar que la categoría y el proveedor existan
        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == producto.IdCategoria);
        if (!categoriaExiste)
        {
            return BadRequest(new { message = "La categoría especificada no existe." });
        }

        var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == producto.IdProveedor);
        if (!proveedorExiste)
        {
            return BadRequest(new { message = "El proveedor especificado no existe." });
        }

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        // Devolvemos el producto con sus relaciones cargadas
        var productoConRelaciones = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == producto.Id);

        return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, productoConRelaciones);
    }

    // PUT: api/productos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProducto(int id, Producto producto)
    {
        if (id != producto.Id)
        {
            return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validar que la categoría y el proveedor existan (si se están actualizando)
        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == producto.IdCategoria);
        if (!categoriaExiste)
        {
            return BadRequest(new { message = "La categoría especificada no existe." });
        }

        var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == producto.IdProveedor);
        if (!proveedorExiste)
        {
            return BadRequest(new { message = "El proveedor especificado no existe." });
        }

        _context.Entry(producto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductoExists(id))
            {
                return NotFound(new { message = "Producto no encontrado." });
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/productos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductoExists(int id)
    {
        return _context.Productos.Any(e => e.Id == id);
    }

    // ========== ENDPOINTS DE AGREGACIÓN (REQUERIDOS PARA LA TAREA) ==========

    /// <summary>
    /// Endpoint único que devuelve estadísticas de precios.
    /// GET: api/productos/estadisticas
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<object>> GetEstadisticasPrecios()
    {
        // Verificar si hay productos
        if (!await _context.Productos.AnyAsync())
        {
            return Ok(new
            {
                message = "No hay productos registrados.",
                precioMasAlto = (decimal?)null,
                precioMasBajo = (decimal?)null,
                sumaTotalPrecios = 0,
                precioPromedio = 0
            });
        }

        var precioMasAlto = await _context.Productos.MaxAsync(p => p.Precio);
        var precioMasBajo = await _context.Productos.MinAsync(p => p.Precio);
        var sumaTotal = await _context.Productos.SumAsync(p => p.Precio);
        var precioPromedio = await _context.Productos.AverageAsync(p => p.Precio);

        // También podemos devolver los productos que tienen esos precios
        var productoMasCaro = await _context.Productos
            .Where(p => p.Precio == precioMasAlto)
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync();

        var productoMasBarato = await _context.Productos
            .Where(p => p.Precio == precioMasBajo)
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync();

        return Ok(new
        {
            precioMasAlto = new { precio = precioMasAlto, producto = productoMasCaro?.Nombre },
            precioMasBajo = new { precio = precioMasBajo, producto = productoMasBarato?.Nombre },
            sumaTotalPrecios = sumaTotal,
            precioPromedio = Math.Round(precioPromedio, 2),
            cantidadTotalProductos = await _context.Productos.CountAsync()
        });
    }

    /// <summary>
    /// Endpoint para obtener productos de una categoría específica.
    /// GET: api/productos/por-categoria/{categoriaId}
    /// </summary>
    [HttpGet("por-categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorCategoria(int categoriaId)
    {
        // Verificar si la categoría existe
        var categoria = await _context.Categorias.FindAsync(categoriaId);
        if (categoria == null)
        {
            return NotFound(new { message = "La categoría especificada no existe." });
        }

        var productos = await _context.Productos
            .Where(p => p.IdCategoria == categoriaId)
            .Include(p => p.Proveedor)
            .Include(p => p.Categoria)
            .ToListAsync();

        return Ok(new
        {
            categoria = categoria.Nombre,
            cantidad = productos.Count,
            productos = productos
        });
    }

    /// <summary>
    /// Endpoint para obtener productos de un proveedor específico.
    /// GET: api/productos/por-proveedor/{proveedorId}
    /// </summary>
    [HttpGet("por-proveedor/{proveedorId}")]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorProveedor(int proveedorId)
    {
        var proveedor = await _context.Proveedores.FindAsync(proveedorId);
        if (proveedor == null)
        {
            return NotFound(new { message = "El proveedor especificado no existe." });
        }

        var productos = await _context.Productos
            .Where(p => p.IdProveedor == proveedorId)
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .ToListAsync();

        return Ok(new
        {
            proveedor = proveedor.Nombre,
            cantidad = productos.Count,
            productos = productos
        });
    }

    /// <summary>
    /// Endpoint para obtener la cantidad total de productos.
    /// GET: api/productos/cantidad-total
    /// </summary>
    [HttpGet("cantidad-total")]
    public async Task<ActionResult<object>> GetCantidadTotalProductos()
    {
        var cantidad = await _context.Productos.CountAsync();
        var stockTotal = await _context.Productos.SumAsync(p => p.Stock);

        return Ok(new
        {
            cantidadTotalProductos = cantidad,
            stockTotal = stockTotal,
            message = cantidad == 0 ? "No hay productos registrados." : null
        });
    }
}