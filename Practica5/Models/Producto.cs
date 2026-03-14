using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practica5.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es requerido.")]
    [MaxLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es requerido.")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    // Claves foráneas
    public int IdProveedor { get; set; }
    public int IdCategoria { get; set; }

    // Relaciones de navegación
    [ForeignKey("IdProveedor")]
    public Proveedor? Proveedor { get; set; }

    [ForeignKey("IdCategoria")]
    public Categoria? Categoria { get; set; }
}