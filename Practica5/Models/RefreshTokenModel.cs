using System.ComponentModel.DataAnnotations;

namespace Practica5.Models;

public class RefreshTokenModel
{
    [Required(ErrorMessage = "El refresh token es requerido")]
    public string RefreshToken { get; set; } = string.Empty;
}
