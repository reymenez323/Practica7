using System.ComponentModel.DataAnnotations;

namespace Practica5.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    // Relación: Una categoría puede tener muchos productos
    public ICollection<Producto>? Productos { get; set; }
}