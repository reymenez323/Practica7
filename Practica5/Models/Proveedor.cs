using System.ComponentModel.DataAnnotations;

namespace Practica5.Models;

public class Proveedor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del proveedor es requerido.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "El contacto no puede exceder los 100 caracteres.")]
    public string? Contacto { get; set; } // Nombre de la persona de contacto o email

    // Relación: Un proveedor puede tener muchos productos
    public ICollection<Producto>? Productos { get; set; }
}