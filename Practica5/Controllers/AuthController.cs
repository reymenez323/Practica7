using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;
using Practica5.Services;

namespace Practica5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginModel loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo.ToLower() == loginModel.Correo.ToLower().Trim());

        if (usuario == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        if (!_tokenService.VerifyPassword(loginModel.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        var accessToken = _tokenService.GenerateAccessToken(usuario);
        var refreshToken = _tokenService.GenerateRefreshToken();

        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _db.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(15),
            Nombre = usuario.Nombre,
            Correo = usuario.Correo
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenModel refreshTokenModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenModel.RefreshToken);

        if (usuario == null)
        {
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        if (usuario.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Refresh token expirado" });
        }

        var newAccessToken = _tokenService.GenerateAccessToken(usuario);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        usuario.RefreshToken = newRefreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _db.SaveChangesAsync();

        return Ok(new TokenResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(15),
            Nombre = usuario.Nombre,
            Correo = usuario.Correo
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<Usuario>> Register(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var correo = usuario.Correo.Trim().ToLowerInvariant();
        var exists = await _db.Usuarios.AnyAsync(u => u.Correo.ToLower() == correo);
        if (exists)
        {
            return BadRequest(new { message = "El correo electrónico ya está en uso." });
        }

        usuario.PasswordHash = _tokenService.HashPassword(usuario.PasswordHash);
        usuario.Correo = correo;
        usuario.FechaDeRegistro = DateTime.Now;

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        usuario.PasswordHash = string.Empty;

        return CreatedAtAction(nameof(GetById), "Usuarios", new { id = usuario.Id }, usuario);
    }

    private async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();
        usuario.PasswordHash = string.Empty;
        return usuario;
    }
}
