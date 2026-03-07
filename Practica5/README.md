# API de Usuarios con Autenticación JWT

API RESTful desarrollada en .NET 10.0 con autenticación JWT, Entity Framework Core y SQL Server.

## 📋 Requisitos previos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server LocalDB](https://docs.microsoft.com/es-es/sql/database-engine/configure-windows/sql-server-express-localdb)

## 🚀 Instalación y ejecución

1. Clonar el repositorio:
`ash
git clone https://github.com/tuusuario/Practica5.git
cd Practica5
Restaurar paquetes NuGet:

bash
dotnet restore
Aplicar migraciones:

bash
dotnet ef database update
Ejecutar la API:

bash
dotnet run
La API estará disponible en:

HTTP: http://localhost:5074

HTTPS: https://localhost:7005

Swagger UI: https://localhost:7005/swagger

📚 Endpoints
Autenticación
EndpointMétodoDescripción
/api/auth/registerPOSTRegistrar nuevo usuario
/api/auth/loginPOSTIniciar sesión y obtener token
/api/auth/refreshPOSTRefrescar token JWT
Usuarios (Protegidos)
EndpointMétodoDescripción
/api/usuariosGETObtener todos los usuarios
/api/usuarios/{id}GETObtener usuario por ID
/api/usuarios/{id}PUTActualizar usuario (solo propio)
/api/usuarios/{id}DELETEEliminar usuario (solo propio)
🔧 Ejemplos de uso
Registrar usuario
json
POST https://localhost:7005/api/auth/register
{
    "nombre": "Juan Pérez",
    "correo": "juan@example.com",
    "passwordHash": "MiPassword123"
}
Iniciar sesión
json
POST https://localhost:7005/api/auth/login
{
    "correo": "juan@example.com",
    "password": "MiPassword123"
}
Obtener usuarios (con token)
text
GET https://localhost:7005/api/usuarios
Headers: Authorization: Bearer {tu-token-aqui}
📦 Paquetes NuGet utilizados
Microsoft.AspNetCore.Authentication.JwtBearer

Microsoft.EntityFrameworkCore.SqlServer

Microsoft.EntityFrameworkCore.Tools

Swashbuckle.AspNetCore

System.IdentityModel.Tokens.Jwt

📝 Validaciones implementadas
✅ Nombre: Required, MaxLength(100), MinLength(2)

✅ Correo: Required, EmailAddress, MaxLength(200)

✅ Password: Required, MinLength(6), MaxLength(100)
